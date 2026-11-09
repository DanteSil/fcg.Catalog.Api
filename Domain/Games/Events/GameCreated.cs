using Fcg.Domain.Common;

namespace Fcg.Domain.Games.Events;

public sealed record GameCreated(Guid GameId, string Title, DateTime OccurredAt) : IDomainEvent;
