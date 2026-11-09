using Fcg.Domain.Common;
using Fcg.Domain.Promotions.Events;

namespace Fcg.Domain.Promotions;

public class Promotion : Entity
{
    public Guid GameId { get; private set; }
    public decimal DiscountPercent { get; private set; }
    public DateTime StartsAt { get; private set; }
    public DateTime EndsAt { get; private set; }

    private Promotion()
    {
    }

    private Promotion(Guid gameId, decimal discountPercent, DateTime startsAt, DateTime endsAt)
    {
        GameId = gameId;
        DiscountPercent = discountPercent;
        StartsAt = startsAt;
        EndsAt = endsAt;
        Raise(new PromotionCreated(Id, gameId, discountPercent, DateTime.UtcNow));
    }

    public static Promotion Create(Guid gameId, decimal discountPercent, DateTime startsAt, DateTime endsAt)
    {
        if (gameId == Guid.Empty)
            throw new DomainException("Jogo inválido.");

        if (discountPercent is <= 0 or > 100)
            throw new DomainException("Desconto deve ser maior que 0 e no máximo 100.");

        if (endsAt <= startsAt)
            throw new DomainException("Data de término deve ser posterior à data de início.");

        return new Promotion(gameId, discountPercent, startsAt, endsAt);
    }

    public void Update(decimal discountPercent, DateTime startsAt, DateTime endsAt)
    {
        if (discountPercent is <= 0 or > 100)
            throw new DomainException("Desconto deve ser maior que 0 e no máximo 100.");

        if (endsAt <= startsAt)
            throw new DomainException("Data de término deve ser posterior à data de início.");

        DiscountPercent = discountPercent;
        StartsAt = startsAt;
        EndsAt = endsAt;
    }

    public bool IsActive(DateTime? at = null)
    {
        var moment = at ?? DateTime.UtcNow;
        return moment >= StartsAt && moment <= EndsAt;
    }
}
