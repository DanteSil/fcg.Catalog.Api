namespace Fcg.Application.Library;

public interface IOrderPublisher
{
    Task PublishAsync(OrderPlacedEvent order, CancellationToken cancellationToken);
}
