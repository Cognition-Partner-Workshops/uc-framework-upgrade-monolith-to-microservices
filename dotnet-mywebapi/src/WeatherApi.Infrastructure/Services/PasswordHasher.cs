using WeatherApi.Application.Common.Interfaces;

namespace WeatherApi.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    // Simple SHA256 hash for demo; in production use BCrypt.Net
    public string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return hash == HashPassword(password);
    }
}
