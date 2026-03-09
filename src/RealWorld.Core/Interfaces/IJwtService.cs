using RealWorld.Core.Entities;

namespace RealWorld.Core.Interfaces;

public interface IJwtService
{
    string ToToken(User user);
    string? GetSubFromToken(string token);
}
