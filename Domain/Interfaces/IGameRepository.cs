using Fcg.Domain.Games;

namespace Fcg.Domain.Interfaces;

public interface IGameRepository
{
    Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Game>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Game game, CancellationToken cancellationToken = default);
    void Update(Game game);
    void Remove(Game game);
}
