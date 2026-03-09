using System.Security.Claims;
using RealWorld.Core.Interfaces;

namespace RealWorld.Api.Security;

public class JwtTokenFilter : IMiddleware
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;

    public JwtTokenFilter(IJwtService jwtService, IUserRepository userRepository)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var header = context.Request.Headers["Authorization"].FirstOrDefault();

        if (header != null)
        {
            var token = ExtractToken(header);
            if (token != null)
            {
                var userId = _jwtService.GetSubFromToken(token);
                if (userId != null)
                {
                    var user = await _userRepository.FindByIdAsync(userId);
                    if (user != null)
                    {
                        // Store user in HttpContext.Items for controller access
                        context.Items["User"] = user;

                        // Set ClaimsPrincipal
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Name, user.Username)
                        };
                        var identity = new ClaimsIdentity(claims, "Bearer");
                        context.User = new ClaimsPrincipal(identity);
                    }
                }
            }
        }

        await next(context);
    }

    private static string? ExtractToken(string header)
    {
        // Support both "Token <jwt>" (RealWorld spec) and "Bearer <jwt>"
        if (header.StartsWith("Token ", StringComparison.OrdinalIgnoreCase))
            return header[6..].Trim();
        if (header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return header[7..].Trim();
        return null;
    }
}
