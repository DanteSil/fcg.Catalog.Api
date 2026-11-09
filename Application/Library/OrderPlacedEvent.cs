namespace Fcg.Application.Library;

public sealed record OrderPlacedEvent(Guid OrderId, Guid UserId, Guid GameId, decimal Price);
