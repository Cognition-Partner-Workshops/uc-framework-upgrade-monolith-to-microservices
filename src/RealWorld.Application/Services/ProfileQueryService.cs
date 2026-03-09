using RealWorld.Application.DTOs;
using RealWorld.Core.Entities;

namespace RealWorld.Application.Services;

public class ProfileQueryService
{
    private readonly IUserReadService _userReadService;
    private readonly IUserRelationshipQueryService _userRelationshipQueryService;

    public ProfileQueryService(
        IUserReadService userReadService,
        IUserRelationshipQueryService userRelationshipQueryService)
    {
        _userReadService = userReadService;
        _userRelationshipQueryService = userRelationshipQueryService;
    }

    public async Task<ProfileData?> FindByUsernameAsync(string username, User? currentUser)
    {
        var userData = await _userReadService.FindByUsernameAsync(username);
        if (userData == null)
            return null;

        bool following = currentUser != null &&
            await _userRelationshipQueryService.IsUserFollowingAsync(currentUser.Id, userData.Id);

        return new ProfileData(
            userData.Id,
            userData.Username,
            userData.Bio,
            userData.Image,
            following);
    }
}
