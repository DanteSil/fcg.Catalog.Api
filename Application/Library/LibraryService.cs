using Fcg.Application.Common;
using Fcg.Domain.Interfaces;
using Fcg.Domain.Library;
using Fcg.Domain.Users;

namespace Fcg.Application.Library;

public class LibraryService
{
    private readonly ILibraryRepository _library;
    private readonly IGameRepository _games;
    private readonly IUnitOfWork _unitOfWork;

    public LibraryService(ILibraryRepository library, IGameRepository games, IUnitOfWork unitOfWork)
    {
        _library = library;
        _games = games;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<LibraryItemDto>> GetMyLibraryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _library.GetByUserIdAsync(userId, cancellationToken);
        var result = new List<LibraryItemDto>();

        foreach (var item in items)
        {
            var game = await _games.GetByIdAsync(item.GameId, cancellationToken);
            if (game is null) continue;

            result.Add(new LibraryItemDto(item.Id, game.Id, game.Title, game.Description, game.Price, item.AcquiredAt));
        }

        return result;
    }

    public async Task<LibraryItemDto> AcquireAsync(Guid userId, Guid gameId, UserRole actorRole, CancellationToken cancellationToken = default)
    {
        if (actorRole == UserRole.Admin)
            throw new ForbiddenException("Administradores não adquirem jogos pela biblioteca de usuário.");

        var game = await _games.GetByIdAsync(gameId, cancellationToken)
            ?? throw new NotFoundException("Jogo não encontrado.");

        if (await _library.ExistsAsync(userId, gameId, cancellationToken))
            throw new ConflictException("Jogo já está na biblioteca.");

        var item = LibraryItem.Acquire(userId, gameId);
        await _library.AddAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LibraryItemDto(item.Id, game.Id, game.Title, game.Description, game.Price, item.AcquiredAt);
    }
}
