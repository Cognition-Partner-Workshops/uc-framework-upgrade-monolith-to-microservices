using RealWorld.Api.Models;

namespace RealWorld.Api.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    string? GetUserIdFromToken(string token);
}
