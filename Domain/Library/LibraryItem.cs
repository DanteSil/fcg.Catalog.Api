using Fcg.Domain.Common;
using Fcg.Domain.Library.Events;

namespace Fcg.Domain.Library;

public class LibraryItem : Entity
{
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public DateTime AcquiredAt { get; private set; }

    private LibraryItem()
    {
    }

    private LibraryItem(Guid userId, Guid gameId)
    {
        UserId = userId;
        GameId = gameId;
        AcquiredAt = DateTime.UtcNow;
        Raise(new GameAcquired(userId, gameId, AcquiredAt));
    }

    public static LibraryItem Acquire(Guid userId, Guid gameId)
    {
        if (userId == Guid.Empty)
            throw new DomainException("Usuário inválido.");

        if (gameId == Guid.Empty)
            throw new DomainException("Jogo inválido.");

        return new LibraryItem(userId, gameId);
    }
}
