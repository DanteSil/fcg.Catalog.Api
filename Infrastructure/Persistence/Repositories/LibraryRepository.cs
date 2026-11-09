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

    public async Task AddAsync(LibraryItem item, CancellationToken cancellationToken = default) =>
        await _db.LibraryItems.AddAsync(item, cancellationToken);
}
