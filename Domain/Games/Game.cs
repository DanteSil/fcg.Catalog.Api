using Fcg.Domain.Common;
using Fcg.Domain.Games.Events;

namespace Fcg.Domain.Games;

public class Game : Entity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Game()
    {
    }

    private Game(string title, string description, decimal price)
    {
        Title = title;
        Description = description;
        Price = price;
        CreatedAt = DateTime.UtcNow;
        Raise(new GameCreated(Id, title, CreatedAt));
    }

    public static Game Create(string title, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Título do jogo é obrigatório.");

        if (price < 0)
            throw new DomainException("Preço não pode ser negativo.");

        return new Game(title.Trim(), description?.Trim() ?? string.Empty, price);
    }

    public void Update(string title, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Título do jogo é obrigatório.");

        if (price < 0)
            throw new DomainException("Preço não pode ser negativo.");

        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
        Price = price;
    }
}
