using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealWorld.Core.Entities;
using RealWorld.Core.Interfaces;

namespace RealWorld.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly string _secret;
    private readonly int _sessionTime;

    public JwtService(IConfiguration configuration)
    {
        _secret = configuration["jwt:secret"]
            ?? Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? throw new ArgumentNullException("jwt:secret configuration or JWT_SECRET env var is missing");
        _sessionTime = int.Parse(configuration["jwt:sessionTime"] ?? "86400");
    }

    private SymmetricSecurityKey GetSigningKey()
    {
        var keyBytes = Encoding.UTF8.GetBytes(_secret);
        // HS512 requires at least 64 bytes (512 bits). If the secret is shorter,
        // pad it with zeros to match Java's SecretKeySpec behavior which doesn't validate key length.
        if (keyBytes.Length < 64)
        {
            var padded = new byte[64];
            Array.Copy(keyBytes, padded, keyBytes.Length);
            keyBytes = padded;
        }
        return new SymmetricSecurityKey(keyBytes);
    }

    public string ToToken(User user)
    {
        var key = GetSigningKey();
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var claims = new[]
        {
            new Claim("sub", user.Id)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(_sessionTime),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string? GetSubFromToken(string token)
    {
        try
        {
            var key = GetSigningKey();
            var handler = new JwtSecurityTokenHandler();
            // Disable automatic claim type mapping so "sub" stays as "sub"
            // instead of being remapped to ClaimTypes.NameIdentifier
            handler.InboundClaimTypeMap.Clear();

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key
            };

            var principal = handler.ValidateToken(token, parameters, out _);
            // Try both "sub" and the mapped NameIdentifier claim type
            return principal.FindFirst("sub")?.Value
                ?? principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }
        catch
        {
            return null;
        }
    }
}
