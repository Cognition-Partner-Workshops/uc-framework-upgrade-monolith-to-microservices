using MediatR;
using Microsoft.AspNetCore.Mvc;
using CqrsApi.Features.Purchases.Commands;
using CqrsApi.Features.Purchases.Queries;

namespace CqrsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PurchasesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all purchases.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var purchases = await _mediator.Send(new GetAllPurchasesQuery());
        return Ok(purchases);
    }

    /// <summary>
    /// Get a purchase by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var purchase = await _mediator.Send(new GetPurchaseByIdQuery(id));
        if (purchase is null)
            return NotFound();

        return Ok(purchase);
    }

    /// <summary>
    /// Create a new purchase.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseCommand command)
    {
        var purchase = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = purchase.Id }, purchase);
    }

    /// <summary>
    /// Update an existing purchase.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePurchaseCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch.");

        var purchase = await _mediator.Send(command);
        if (purchase is null)
            return NotFound();

        return Ok(purchase);
    }

    /// <summary>
    /// Delete a purchase by ID.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeletePurchaseCommand(id));
        if (!result)
            return NotFound();

        return NoContent();
    }
}
