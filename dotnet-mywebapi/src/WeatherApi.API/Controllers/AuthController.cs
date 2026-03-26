using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherApi.API.Middleware;
using WeatherApi.Application.DTOs;
using WeatherApi.Application.Features.Auth.Commands;

namespace WeatherApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new user.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Register), new { id = result.Id }, result);
    }

    /// <summary>
    /// Authenticate and receive a JWT token.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
