using FluentValidation;

namespace Fcg.Application.Promotions;

public class CreatePromotionRequestValidator : AbstractValidator<CreatePromotionRequest>
{
    public CreatePromotionRequestValidator()
    {
        RuleFor(x => x.GameId).NotEmpty();
        RuleFor(x => x.DiscountPercent).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.EndsAt).GreaterThan(x => x.StartsAt);
    }
}

public class UpdatePromotionRequestValidator : AbstractValidator<UpdatePromotionRequest>
{
    public UpdatePromotionRequestValidator()
    {
        RuleFor(x => x.DiscountPercent).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.EndsAt).GreaterThan(x => x.StartsAt);
    }
}
