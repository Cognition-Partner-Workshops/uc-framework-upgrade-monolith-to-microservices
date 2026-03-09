using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Application.DTOs;
using RealWorld.Application.Services;
using RealWorld.Core.Entities;
using RealWorld.Core.Interfaces;

namespace RealWorld.Api.Controllers;

[ApiController]
public class CurrentUserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly UserService _userService;
    private readonly UserQueryService _userQueryService;
    private readonly IJwtService _jwtService;

    public CurrentUserController(
        IUserRepository userRepository,
        UserService userService,
        UserQueryService userQueryService,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _userService = userService;
        _userQueryService = userQueryService;
        _jwtService = jwtService;
    }

    [HttpGet("/user")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = GetUser();
        var userData = await _userQueryService.FindByIdAsync(user.Id);
        var token = _jwtService.ToToken(user);
        return Ok(new { user = new UserWithToken(userData!, token) });
    }

    [HttpPut("/user")]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserParam param)
    {
        var user = GetUser();

        string? hashedPassword = null;
        if (!string.IsNullOrWhiteSpace(param.User.Password))
            hashedPassword = BCrypt.Net.BCrypt.HashPassword(param.User.Password);

        await _userService.UpdateUserAsync(user, param.User.Email, param.User.Username, hashedPassword, param.User.Bio, param.User.Image);

        var updatedUser = await _userRepository.FindByIdAsync(user.Id);
        var userData = await _userQueryService.FindByIdAsync(updatedUser!.Id);
        var token = _jwtService.ToToken(updatedUser);
        return Ok(new { user = new UserWithToken(userData!, token) });
    }

    private User GetUser()
    {
        return HttpContext.Items["User"] as User
            ?? throw new UnauthorizedAccessException("Authentication required");
    }

    public class UpdateUserParam
    {
        [JsonPropertyName("user")]
        public UpdateUserData User { get; set; } = new();
    }

    public class UpdateUserData
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Bio { get; set; }
        public string? Image { get; set; }
    }
}
