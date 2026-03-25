using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherApi.Application.Features.HelloWorld.Queries;

namespace WeatherApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloWorldController : ControllerBase
{
    private readonly IMediator _mediator;

    public HelloWorldController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns a Hello World message (public endpoint).
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetHelloWorldQuery());
        return Ok(result);
    }

    /// <summary>
    /// Returns a Hello World message (requires authentication).
    /// </summary>
    [HttpGet("secure")]
    [Authorize]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSecure()
    {
        var result = await _mediator.Send(new GetHelloWorldQuery());
        return Ok($"{result} - Authenticated as {User.Identity?.Name}");
    }

    /// <summary>
    /// Returns a Hello World message (requires Admin role).
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAdmin()
    {
        var result = await _mediator.Send(new GetHelloWorldQuery());
        return Ok($"{result} - Admin Access Granted");
    }
}
