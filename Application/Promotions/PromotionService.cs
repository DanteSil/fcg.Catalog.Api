using Fcg.Application.Common;
using Fcg.Domain.Interfaces;
using Fcg.Domain.Promotions;
using Fcg.Domain.Users;

namespace Fcg.Application.Promotions;

public class PromotionService
{
    private readonly IPromotionRepository _promotions;
    private readonly IGameRepository _games;
    private readonly IUnitOfWork _unitOfWork;

    public PromotionService(IPromotionRepository promotions, IGameRepository games, IUnitOfWork unitOfWork)
    {
        _promotions = promotions;
        _games = games;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<PromotionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var promotions = await _promotions.GetAllAsync(cancellationToken);
        return promotions.Select(p => p.ToDto()).ToList();
    }

    public async Task<PromotionDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var promotion = await _promotions.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Promoção não encontrada.");

        return promotion.ToDto();
    }

    public async Task<PromotionDto> CreateAsync(CreatePromotionRequest request, UserRole actorRole, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(actorRole);

        _ = await _games.GetByIdAsync(request.GameId, cancellationToken)
            ?? throw new NotFoundException("Jogo não encontrado.");

        var promotion = Promotion.Create(request.GameId, request.DiscountPercent, request.StartsAt, request.EndsAt);
        await _promotions.AddAsync(promotion, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return promotion.ToDto();
    }

    public async Task<PromotionDto> UpdateAsync(Guid id, UpdatePromotionRequest request, UserRole actorRole, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(actorRole);

        var promotion = await _promotions.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Promoção não encontrada.");

        promotion.Update(request.DiscountPercent, request.StartsAt, request.EndsAt);
        _promotions.Update(promotion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return promotion.ToDto();
    }

    public async Task DeleteAsync(Guid id, UserRole actorRole, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(actorRole);

        var promotion = await _promotions.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Promoção não encontrada.");

        _promotions.Remove(promotion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureAdmin(UserRole role)
    {
        if (role != UserRole.Admin)
            throw new ForbiddenException("Apenas administradores podem gerenciar promoções.");
    }
}
