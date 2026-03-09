using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealWorld.Api.DTOs;
using RealWorld.Api.Infrastructure.Data;
using RealWorld.Api.Models;
using RealWorld.Api.Services;

namespace RealWorld.Api.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;

    public UserController(AppDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    private User? GetAuthenticatedUser() => HttpContext.Items["User"] as User;

    [HttpGet]
    public IActionResult GetCurrentUser()
    {
        var user = GetAuthenticatedUser();
        if (user == null) return Unauthorized();

        var token = Request.Headers["Authorization"].ToString().Split(' ').LastOrDefault() ?? "";

        return Ok(new UserResponse
        {
            User = new UserDataDto
            {
                Email = user.Email,
                Username = user.Username,
                Bio = user.Bio,
                Image = user.Image,
                Token = token
            }
        });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        var user = GetAuthenticatedUser();
        if (user == null) return Unauthorized();

        var dto = request.User;

        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return UnprocessableEntity(new { errors = new { email = new[] { "has already been taken" } } });
        }

        if (!string.IsNullOrEmpty(dto.Username) && dto.Username != user.Username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return UnprocessableEntity(new { errors = new { username = new[] { "has already been taken" } } });
        }

        var password = !string.IsNullOrEmpty(dto.Password)
            ? BCrypt.Net.BCrypt.HashPassword(dto.Password)
            : null;

        user.Update(dto.Email, dto.Username, password, dto.Bio, dto.Image);

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        var token = Request.Headers["Authorization"].ToString().Split(' ').LastOrDefault() ?? "";

        return Ok(new UserResponse
        {
            User = new UserDataDto
            {
                Email = user.Email,
                Username = user.Username,
                Bio = user.Bio,
                Image = user.Image,
                Token = token
            }
        });
    }
}
