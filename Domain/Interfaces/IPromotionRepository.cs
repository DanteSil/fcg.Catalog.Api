using Fcg.Domain.Promotions;

namespace Fcg.Domain.Interfaces;

public interface IPromotionRepository
{
    Task<Promotion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Promotion>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Promotion promotion, CancellationToken cancellationToken = default);
    void Update(Promotion promotion);
    void Remove(Promotion promotion);
}
