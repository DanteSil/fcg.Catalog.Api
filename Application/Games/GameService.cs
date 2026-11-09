using Fcg.Application.Common;
using Fcg.Domain.Games;
using Fcg.Domain.Interfaces;
using Fcg.Domain.Users;

namespace Fcg.Application.Games;

public class GameService
{
    private readonly IGameRepository _games;
    private readonly IUnitOfWork _unitOfWork;

    public GameService(IGameRepository games, IUnitOfWork unitOfWork)
    {
        _games = games;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<GameDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var games = await _games.GetAllAsync(cancellationToken);
        return games.Select(g => g.ToDto()).ToList();
    }

    public async Task<GameDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var game = await _games.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Jogo não encontrado.");

        return game.ToDto();
    }

    public async Task<GameDto> CreateAsync(CreateGameRequest request, UserRole actorRole, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(actorRole);

        var game = Game.Create(request.Title, request.Description, request.Price);
        await _games.AddAsync(game, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return game.ToDto();
    }

    public async Task<GameDto> UpdateAsync(Guid id, UpdateGameRequest request, UserRole actorRole, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(actorRole);

        var game = await _games.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Jogo não encontrado.");

        game.Update(request.Title, request.Description, request.Price);
        _games.Update(game);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return game.ToDto();
    }

    public async Task DeleteAsync(Guid id, UserRole actorRole, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(actorRole);

        var game = await _games.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Jogo não encontrado.");

        _games.Remove(game);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureAdmin(UserRole role)
    {
        if (role != UserRole.Admin)
            throw new ForbiddenException("Apenas administradores podem gerenciar jogos.");
    }
}
