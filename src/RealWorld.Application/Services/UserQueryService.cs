using RealWorld.Application.DTOs;

namespace RealWorld.Application.Services;

public class UserQueryService
{
    private readonly IUserReadService _userReadService;

    public UserQueryService(IUserReadService userReadService)
    {
        _userReadService = userReadService;
    }

    public async Task<UserData?> FindByIdAsync(string id)
    {
        return await _userReadService.FindByIdAsync(id);
    }
}
