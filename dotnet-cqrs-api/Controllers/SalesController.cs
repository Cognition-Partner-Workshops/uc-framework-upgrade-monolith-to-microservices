using MediatR;
using Microsoft.AspNetCore.Mvc;
using CqrsApi.Features.Sales.Commands;
using CqrsApi.Features.Sales.Queries;

namespace CqrsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all sales.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sales = await _mediator.Send(new GetAllSalesQuery());
        return Ok(sales);
    }

    /// <summary>
    /// Get a sale by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sale = await _mediator.Send(new GetSaleByIdQuery(id));
        if (sale is null)
            return NotFound();

        return Ok(sale);
    }

    /// <summary>
    /// Create a new sale.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleCommand command)
    {
        var sale = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
    }

    /// <summary>
    /// Update an existing sale.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSaleCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch.");

        var sale = await _mediator.Send(command);
        if (sale is null)
            return NotFound();

        return Ok(sale);
    }

    /// <summary>
    /// Delete a sale by ID.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteSaleCommand(id));
        if (!result)
            return NotFound();

        return NoContent();
    }
}
