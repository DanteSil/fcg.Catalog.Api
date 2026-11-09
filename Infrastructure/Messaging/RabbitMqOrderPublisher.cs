using System.Text.Json;
using Fcg.Application.Library;
using RabbitMQ.Client;

namespace Fcg.Infrastructure.Messaging;

public sealed class RabbitMqOrderPublisher(IConfiguration configuration) : IOrderPublisher
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task PublishAsync(OrderPlacedEvent order, CancellationToken cancellationToken)
    {
        var uri = configuration["RabbitMQ:Uri"];
        if (string.IsNullOrWhiteSpace(uri))
            throw new InvalidOperationException("Configure RabbitMQ:Uri para publicar pedidos.");

        var factory = new ConnectionFactory
        {
            Uri = new Uri(uri),
            ClientProvidedName = "fcg-catalog"
        };

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        var options = new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true);
        await using var channel = await connection.CreateChannelAsync(options, cancellationToken);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            Persistent = true,
            MessageId = order.OrderId.ToString(),
            Type = nameof(OrderPlacedEvent)
        };
        var body = JsonSerializer.SerializeToUtf8Bytes(order, JsonOptions);
        await channel.BasicPublishAsync("pedidos", "pedido.criado", mandatory: true, basicProperties: properties,
            body: body, cancellationToken: cancellationToken);
    }
}
