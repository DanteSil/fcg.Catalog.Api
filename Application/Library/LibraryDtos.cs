namespace Fcg.Application.Library;

public record LibraryItemDto(Guid Id, Guid GameId, string Title, string Description, decimal Price, DateTime AcquiredAt);

public record AcquireGameRequest(Guid GameId);
