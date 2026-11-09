using Microsoft.EntityFrameworkCore;
using Fcg.Domain.Games;
using Fcg.Domain.Interfaces;

namespace Fcg.Infrastructure.Persistence.Repositories;

public class GameRepository : IGameRepository
{
    private readonly FcgDbContext _db;

    public GameRepository(FcgDbContext db) => _db = db;

    public async Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _db.Games.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Game>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.Games.OrderBy(x => x.Title).ToListAsync(cancellationToken);

    public async Task AddAsync(Game game, CancellationToken cancellationToken = default) =>
        await _db.Games.AddAsync(game, cancellationToken);

    public void Update(Game game) => _db.Games.Update(game);

    public void Remove(Game game) => _db.Games.Remove(game);
}
