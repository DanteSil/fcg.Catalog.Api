using Fcg.Domain.Library;

namespace Fcg.Domain.Interfaces;

public interface ILibraryRepository
{
    Task<IReadOnlyList<LibraryItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid userId, Guid gameId, CancellationToken cancellationToken = default);
    Task AddAsync(LibraryItem item, CancellationToken cancellationToken = default);
}
