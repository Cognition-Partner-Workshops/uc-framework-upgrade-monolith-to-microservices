using RealWorld.Api.Infrastructure.Data;
using RealWorld.Api.Services;

namespace RealWorld.Api.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IJwtService jwtService, AppDbContext dbContext)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null)
        {
            var parts = authHeader.Split(' ');
            if (parts.Length == 2)
            {
                var token = parts[1];
                var userId = jwtService.GetUserIdFromToken(token);
                if (userId != null)
                {
                    var user = await dbContext.Users.FindAsync(userId);
                    if (user != null)
                    {
                        context.Items["User"] = user;
                    }
                }
            }
        }

        await _next(context);
    }
}
