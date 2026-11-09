using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fcg.CatalogAPI.Extensions;
using Fcg.Application.Library;

namespace Fcg.CatalogAPI.Controllers;

[ApiController]
[Route("api/library")]
[Authorize]
public class LibraryController : ControllerBase
{
    private readonly LibraryService _libraryService;

    public LibraryController(LibraryService libraryService) => _libraryService = libraryService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LibraryItemDto>>> GetMine(CancellationToken cancellationToken)
    {
        return Ok(await _libraryService.GetMyLibraryAsync(User.GetUserId(), cancellationToken));
    }

    [HttpPost("{gameId:guid}")]
    public async Task<ActionResult<LibraryItemDto>> Acquire(Guid gameId, CancellationToken cancellationToken)
    {
        var item = await _libraryService.AcquireAsync(User.GetUserId(), gameId, User.GetUserRole(), cancellationToken);
        return Created(string.Empty, item);
    }
}
