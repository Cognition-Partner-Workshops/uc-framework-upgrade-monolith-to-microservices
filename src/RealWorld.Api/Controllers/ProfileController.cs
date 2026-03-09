using Microsoft.AspNetCore.Mvc;
using RealWorld.Api.Exceptions;
using RealWorld.Application.Services;
using RealWorld.Core.Entities;
using RealWorld.Core.Interfaces;

namespace RealWorld.Api.Controllers;

[ApiController]
public class ProfileController : ControllerBase
{
    private readonly ProfileQueryService _profileQueryService;
    private readonly IUserRepository _userRepository;

    public ProfileController(
        ProfileQueryService profileQueryService,
        IUserRepository userRepository)
    {
        _profileQueryService = profileQueryService;
        _userRepository = userRepository;
    }

    [HttpGet("/profiles/{username}")]
    public async Task<IActionResult> GetProfile(string username)
    {
        var user = GetOptionalUser();
        var profile = await _profileQueryService.FindByUsernameAsync(username, user);
        if (profile == null)
            throw new ResourceNotFoundException();
        return Ok(new { profile });
    }

    [HttpPost("/profiles/{username}/follow")]
    public async Task<IActionResult> Follow(string username)
    {
        var user = GetUser();
        var target = await _userRepository.FindByUsernameAsync(username);
        if (target == null)
            throw new ResourceNotFoundException();

        var relation = new FollowRelation(user.Id, target.Id);
        await _userRepository.SaveRelationAsync(relation);

        var profile = await _profileQueryService.FindByUsernameAsync(username, user);
        return Ok(new { profile });
    }

    [HttpDelete("/profiles/{username}/follow")]
    public async Task<IActionResult> Unfollow(string username)
    {
        var user = GetUser();
        var target = await _userRepository.FindByUsernameAsync(username);
        if (target == null)
            throw new ResourceNotFoundException();

        var relation = await _userRepository.FindRelationAsync(user.Id, target.Id);
        if (relation != null)
            await _userRepository.RemoveRelationAsync(relation);

        var profile = await _profileQueryService.FindByUsernameAsync(username, user);
        return Ok(new { profile });
    }

    private User? GetOptionalUser()
    {
        return HttpContext.Items["User"] as User;
    }

    private User GetUser()
    {
        return HttpContext.Items["User"] as User
            ?? throw new UnauthorizedAccessException("Authentication required");
    }
}
