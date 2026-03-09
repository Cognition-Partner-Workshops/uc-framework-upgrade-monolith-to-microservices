using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Api.Exceptions;
using RealWorld.Application.DTOs;
using RealWorld.Application.Services;
using RealWorld.Core.Entities;
using RealWorld.Core.Interfaces;

namespace RealWorld.Api.Controllers;

[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly UserService _userService;
    private readonly UserQueryService _userQueryService;
    private readonly IJwtService _jwtService;
    private readonly string _defaultImage;

    public UsersController(
        IUserRepository userRepository,
        UserService userService,
        UserQueryService userQueryService,
        IJwtService jwtService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _userService = userService;
        _userQueryService = userQueryService;
        _jwtService = jwtService;
        _defaultImage = configuration["image:default"] ?? "https://static.productionready.io/images/smiley-cyrus.jpg";
    }

    [HttpPost("/users")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterParam param)
    {
        var user = await _userService.CreateUserAsync(
            param.User.Email,
            param.User.Username,
            BCrypt.Net.BCrypt.HashPassword(param.User.Password));

        var userData = await _userQueryService.FindByIdAsync(user.Id);
        var token = _jwtService.ToToken(user);
        return Ok(new { user = new UserWithToken(userData!, token) });
    }

    [HttpPost("/users/login")]
    public async Task<IActionResult> Login([FromBody] LoginParam param)
    {
        var user = await _userRepository.FindByEmailAsync(param.User.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(param.User.Password, user.Password))
            throw new InvalidAuthenticationException("invalid email or password");

        var userData = await _userQueryService.FindByIdAsync(user.Id);
        var token = _jwtService.ToToken(user);
        return Ok(new { user = new UserWithToken(userData!, token) });
    }

    public class RegisterParam
    {
        [JsonPropertyName("user")]
        public RegisterUserData User { get; set; } = new();
    }

    public class RegisterUserData
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginParam
    {
        [JsonPropertyName("user")]
        public LoginUserData User { get; set; } = new();
    }

    public class LoginUserData
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
