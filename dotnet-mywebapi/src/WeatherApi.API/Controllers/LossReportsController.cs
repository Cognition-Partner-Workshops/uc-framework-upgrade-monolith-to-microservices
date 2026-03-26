using Microsoft.AspNetCore.Mvc;
using WeatherApi.Application.Common.Interfaces;
using WeatherApi.Application.DTOs;

namespace WeatherApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LossReportsController : ControllerBase
{
    private readonly ILossReportService _lossReportService;
    private readonly ILogger<LossReportsController> _logger;

    public LossReportsController(ILossReportService lossReportService, ILogger<LossReportsController> logger)
    {
        _lossReportService = lossReportService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LossReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LossReportDto>>> GetAll()
    {
        _logger.LogInformation("Getting all loss reports");
        var reports = await _lossReportService.GetAllAsync();
        return Ok(reports);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LossReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LossReportDto>> GetById(int id)
    {
        _logger.LogInformation("Getting loss report with ID {Id}", id);
        var report = await _lossReportService.GetByIdAsync(id);

        if (report == null)
        {
            _logger.LogWarning("Loss report with ID {Id} not found", id);
            return NotFound(new { message = $"Loss report with ID {id} not found." });
        }

        return Ok(report);
    }

    [HttpPost]
    [ProducesResponseType(typeof(LossReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LossReportDto>> Create([FromBody] CreateLossReportDto dto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for creating loss report");
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creating new loss report for policy {PolicyNumber}", dto.PolicyNumber);
        var created = await _lossReportService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(LossReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LossReportDto>> Update(int id, [FromBody] UpdateLossReportDto dto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for updating loss report {Id}", id);
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Updating loss report with ID {Id}", id);
        var updated = await _lossReportService.UpdateAsync(id, dto);

        if (updated == null)
        {
            _logger.LogWarning("Loss report with ID {Id} not found for update", id);
            return NotFound(new { message = $"Loss report with ID {id} not found." });
        }

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting loss report with ID {Id}", id);
        var deleted = await _lossReportService.DeleteAsync(id);

        if (!deleted)
        {
            _logger.LogWarning("Loss report with ID {Id} not found for deletion", id);
            return NotFound(new { message = $"Loss report with ID {id} not found." });
        }

        return NoContent();
    }
}
