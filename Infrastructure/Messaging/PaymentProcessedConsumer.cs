using System.Text.Json;
using Fcg.Domain.Interfaces;
using Fcg.Domain.Library;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Fcg.Infrastructure.Messaging;

public sealed class PaymentProcessedConsumer(
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory,
    ILogger<PaymentProcessedConsumer> logger) : BackgroundService
{
    private const string QueueName = "pagamentos.catalogo";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(configuration["RabbitMQ:Uri"]!),
            ClientProvidedName = "fcg-catalog-payments"
        };

        await using var connection = await ConnectAsync(factory, stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await channel.ExchangeDeclareAsync("pagamentos", ExchangeType.Direct, durable: true, cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await channel.QueueBindAsync(QueueName, "pagamentos", "pagamento.processado", cancellationToken: stoppingToken);
        await channel.BasicQosAsync(0, 1, global: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, delivery) => HandleAsync(channel, delivery);
        await channel.BasicConsumeAsync(QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        logger.LogInformation("Consumindo a fila {Queue}", QueueName);

        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }

    private async Task<IConnection> ConnectAsync(ConnectionFactory factory, CancellationToken cancellationToken)
    {
        while (true)
        {
            try
            {
                return await factory.CreateConnectionAsync(cancellationToken);
            }
            catch (BrokerUnreachableException exception) when (!cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(exception, "RabbitMQ indisponível. Nova tentativa em cinco segundos.");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }

    private async Task HandleAsync(IChannel channel, BasicDeliverEventArgs delivery)
    {
        var cancellationToken = delivery.CancellationToken;
        PaymentProcessedEvent? payment;
        try
        {
            payment = JsonSerializer.Deserialize<PaymentProcessedEvent>(delivery.Body.Span, JsonOptions);
        }
        catch (JsonException)
        {
            payment = null;
        }

        if (payment is null || payment.OrderId == Guid.Empty || payment.UserId == Guid.Empty ||
            payment.GameId == Guid.Empty || payment.Status is not ("Approved" or "Rejected"))
        {
            logger.LogWarning("Resultado de pagamento inválido recebido em {Queue}", QueueName);
            await channel.BasicRejectAsync(delivery.DeliveryTag, requeue: false, cancellationToken: cancellationToken);
            return;
        }

        if (payment.Status == "Approved")
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var library = scope.ServiceProvider.GetRequiredService<ILibraryRepository>();
                var item = LibraryItem.Acquire(payment.UserId, payment.GameId);
                if (await library.AddIfMissingAsync(item, cancellationToken))
                    logger.LogInformation("Jogo incluído na biblioteca para o pedido {OrderId}", payment.OrderId);
            }
            catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
            {
                logger.LogError(exception, "Falha ao atualizar a biblioteca para o pedido {OrderId}", payment.OrderId);
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                await channel.BasicNackAsync(delivery.DeliveryTag, multiple: false, requeue: true, cancellationToken: cancellationToken);
                return;
            }
        }

        await channel.BasicAckAsync(delivery.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
    }

    private sealed record PaymentProcessedEvent(Guid OrderId, Guid UserId, Guid GameId, string Status);
}
