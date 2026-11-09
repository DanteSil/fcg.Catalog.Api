using Fcg.Domain.Common;

namespace Fcg.Domain.Promotions.Events;

public sealed record PromotionCreated(Guid PromotionId, Guid GameId, decimal DiscountPercent, DateTime OccurredAt) : IDomainEvent;
