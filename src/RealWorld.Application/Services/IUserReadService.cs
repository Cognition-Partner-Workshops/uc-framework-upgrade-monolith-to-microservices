using RealWorld.Application.DTOs;

namespace RealWorld.Application.Services;

public interface IUserReadService
{
    Task<UserData?> FindByUsernameAsync(string username);
    Task<UserData?> FindByIdAsync(string id);
}
