using Microsoft.EntityFrameworkCore;
using Fcg.Domain.Interfaces;
using Fcg.Domain.Promotions;

namespace Fcg.Infrastructure.Persistence.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly FcgDbContext _db;

    public PromotionRepository(FcgDbContext db) => _db = db;

    public async Task<Promotion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _db.Promotions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Promotion>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.Promotions.OrderByDescending(x => x.StartsAt).ToListAsync(cancellationToken);

    public async Task AddAsync(Promotion promotion, CancellationToken cancellationToken = default) =>
        await _db.Promotions.AddAsync(promotion, cancellationToken);

    public void Update(Promotion promotion) => _db.Promotions.Update(promotion);

    public void Remove(Promotion promotion) => _db.Promotions.Remove(promotion);
}
