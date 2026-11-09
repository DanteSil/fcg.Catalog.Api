using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fcg.CatalogAPI.Extensions;
using Fcg.Application.Promotions;

namespace Fcg.CatalogAPI.Controllers;

[ApiController]
[Route("api/promotions")]
[Authorize]
public class PromotionsController : ControllerBase
{
    private readonly PromotionService _promotionService;
    private readonly IValidator<CreatePromotionRequest> _createValidator;
    private readonly IValidator<UpdatePromotionRequest> _updateValidator;

    public PromotionsController(
        PromotionService promotionService,
        IValidator<CreatePromotionRequest> createValidator,
        IValidator<UpdatePromotionRequest> updateValidator)
    {
        _promotionService = promotionService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PromotionDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _promotionService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PromotionDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _promotionService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PromotionDto>> Create([FromBody] CreatePromotionRequest request, CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);
        var promotion = await _promotionService.CreateAsync(request, User.GetUserRole(), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = promotion.Id }, promotion);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PromotionDto>> Update(Guid id, [FromBody] UpdatePromotionRequest request, CancellationToken cancellationToken)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);
        return Ok(await _promotionService.UpdateAsync(id, request, User.GetUserRole(), cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _promotionService.DeleteAsync(id, User.GetUserRole(), cancellationToken);
        return NoContent();
    }
}
