using Fcg.Domain.Promotions;

namespace Fcg.Application.Promotions;

public record PromotionDto(Guid Id, Guid GameId, decimal DiscountPercent, DateTime StartsAt, DateTime EndsAt, bool IsActive);

public record CreatePromotionRequest(Guid GameId, decimal DiscountPercent, DateTime StartsAt, DateTime EndsAt);

public record UpdatePromotionRequest(decimal DiscountPercent, DateTime StartsAt, DateTime EndsAt);

public static class PromotionMappings
{
    public static PromotionDto ToDto(this Promotion promotion) =>
        new(promotion.Id, promotion.GameId, promotion.DiscountPercent, promotion.StartsAt, promotion.EndsAt, promotion.IsActive());
}
