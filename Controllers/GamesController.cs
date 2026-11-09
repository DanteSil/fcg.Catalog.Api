using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fcg.CatalogAPI.Extensions;
using Fcg.Application.Games;

namespace Fcg.CatalogAPI.Controllers;

[ApiController]
[Route("api/games")]
[Authorize]
public class GamesController : ControllerBase
{
    private readonly GameService _gameService;
    private readonly IValidator<CreateGameRequest> _createValidator;
    private readonly IValidator<UpdateGameRequest> _updateValidator;

    public GamesController(
        GameService gameService,
        IValidator<CreateGameRequest> createValidator,
        IValidator<UpdateGameRequest> updateValidator)
    {
        _gameService = gameService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GameDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _gameService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GameDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _gameService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GameDto>> Create([FromBody] CreateGameRequest request, CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);
        var game = await _gameService.CreateAsync(request, User.GetUserRole(), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = game.Id }, game);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GameDto>> Update(Guid id, [FromBody] UpdateGameRequest request, CancellationToken cancellationToken)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);
        return Ok(await _gameService.UpdateAsync(id, request, User.GetUserRole(), cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _gameService.DeleteAsync(id, User.GetUserRole(), cancellationToken);
        return NoContent();
    }
}
