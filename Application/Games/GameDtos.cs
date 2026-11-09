using Fcg.Domain.Games;

namespace Fcg.Application.Games;

public record GameDto(Guid Id, string Title, string Description, decimal Price, DateTime CreatedAt);

public record CreateGameRequest(string Title, string Description, decimal Price);

public record UpdateGameRequest(string Title, string Description, decimal Price);

public static class GameMappings
{
    public static GameDto ToDto(this Game game) =>
        new(game.Id, game.Title, game.Description, game.Price, game.CreatedAt);
}
