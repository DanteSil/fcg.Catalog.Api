using Microsoft.EntityFrameworkCore;
using Fcg.Domain.Interfaces;
using Fcg.Domain.Library;

namespace Fcg.Infrastructure.Persistence.Repositories;

public class LibraryRepository : ILibraryRepository
{
    private readonly FcgDbContext _db;

    public LibraryRepository(FcgDbContext db) => _db = db;

    public async Task<IReadOnlyList<LibraryItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _db.LibraryItems.Where(x => x.UserId == userId).OrderByDescending(x => x.AcquiredAt).ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(Guid userId, Guid gameId, CancellationToken cancellationToken = default) =>
        await _db.LibraryItems.AnyAsync(x => x.UserId == userId && x.GameId == gameId, cancellationToken);

    public async Task<bool> AddIfMissingAsync(LibraryItem item, CancellationToken cancellationToken = default)
    {
        var inserted = await _db.Database.ExecuteSqlInterpolatedAsync($"""
            INSERT INTO library_items ("Id", "UserId", "GameId", "AcquiredAt")
            VALUES ({item.Id}, {item.UserId}, {item.GameId}, {item.AcquiredAt})
            ON CONFLICT ("UserId", "GameId") DO NOTHING
            """, cancellationToken);

        return inserted == 1;
    }
}
