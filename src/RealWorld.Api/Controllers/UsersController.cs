using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealWorld.Api.DTOs;
using RealWorld.Api.Infrastructure.Data;
using RealWorld.Api.Models;
using RealWorld.Api.Services;

namespace RealWorld.Api.Controllers;

[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public UsersController(AppDbContext context, IJwtService jwtService, IConfiguration configuration)
    {
        _context = context;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    [HttpPost("/users")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var dto = request.User;

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return UnprocessableEntity(new { errors = new { email = new[] { "has already been taken" } } });

        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            return UnprocessableEntity(new { errors = new { username = new[] { "has already been taken" } } });

        var user = new User
        {
            Email = dto.Email,
            Username = dto.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Bio = "",
            Image = _configuration["Image:Default"] ?? "https://static.productionready.io/images/smiley-cyrus.jpg"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return StatusCode(201, new UserResponse
        {
            User = new UserDataDto
            {
                Email = user.Email,
                Username = user.Username,
                Bio = user.Bio,
                Image = user.Image,
                Token = _jwtService.GenerateToken(user)
            }
        });
    }

    [HttpPost("/users/login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var dto = request.User;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            return UnprocessableEntity(new { message = "invalid email or password" });

        return Ok(new UserResponse
        {
            User = new UserDataDto
            {
                Email = user.Email,
                Username = user.Username,
                Bio = user.Bio,
                Image = user.Image,
                Token = _jwtService.GenerateToken(user)
            }
        });
    }
}
