using Fcg.Domain.Common;

namespace Fcg.Domain.Library.Events;

public sealed record GameAcquired(Guid UserId, Guid GameId, DateTime OccurredAt) : IDomainEvent;
