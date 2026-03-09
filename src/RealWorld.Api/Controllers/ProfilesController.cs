using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealWorld.Api.DTOs;
using RealWorld.Api.Infrastructure.Data;
using RealWorld.Api.Models;

namespace RealWorld.Api.Controllers;

[ApiController]
[Route("profiles/{username}")]
public class ProfilesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProfilesController(AppDbContext context)
    {
        _context = context;
    }

    private User? GetCurrentUser() => HttpContext.Items["User"] as User;

    [HttpGet]
    public async Task<IActionResult> GetProfile(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return NotFound();

        var currentUser = GetCurrentUser();
        var following = false;
        if (currentUser != null)
        {
            following = await _context.Follows.AnyAsync(f => f.UserId == currentUser.Id && f.TargetId == user.Id);
        }

        return Ok(new ProfileResponse
        {
            Profile = new ProfileDataDto
            {
                Username = user.Username,
                Bio = user.Bio,
                Image = user.Image,
                Following = following
            }
        });
    }

    [HttpPost("follow")]
    public async Task<IActionResult> Follow(string username)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null) return Unauthorized();

        var target = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (target == null) return NotFound();

        var exists = await _context.Follows.AnyAsync(f => f.UserId == currentUser.Id && f.TargetId == target.Id);
        if (!exists)
        {
            _context.Follows.Add(new FollowRelation { UserId = currentUser.Id, TargetId = target.Id });
            await _context.SaveChangesAsync();
        }

        return Ok(new ProfileResponse
        {
            Profile = new ProfileDataDto
            {
                Username = target.Username,
                Bio = target.Bio,
                Image = target.Image,
                Following = true
            }
        });
    }

    [HttpDelete("follow")]
    public async Task<IActionResult> Unfollow(string username)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null) return Unauthorized();

        var target = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (target == null) return NotFound();

        var relation = await _context.Follows.FirstOrDefaultAsync(f => f.UserId == currentUser.Id && f.TargetId == target.Id);
        if (relation == null) return NotFound();

        _context.Follows.Remove(relation);
        await _context.SaveChangesAsync();

        return Ok(new ProfileResponse
        {
            Profile = new ProfileDataDto
            {
                Username = target.Username,
                Bio = target.Bio,
                Image = target.Image,
                Following = false
            }
        });
    }
}
