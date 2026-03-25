using WeatherApi.Domain.Entities;

namespace WeatherApi.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
