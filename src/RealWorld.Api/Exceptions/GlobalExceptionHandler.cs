using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace RealWorld.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, errors) = exception switch
        {
            ResourceNotFoundException => (HttpStatusCode.NotFound, new { errors = new { body = new[] { exception.Message } } }),
            NoAuthorizationException => (HttpStatusCode.Forbidden, new { errors = new { body = new[] { exception.Message } } }),
            InvalidAuthenticationException => ((HttpStatusCode)422, new { errors = new { body = new[] { exception.Message } } }),
            InvalidOperationException => ((HttpStatusCode)422, new { errors = new { body = new[] { exception.Message } } }),
            _ => (HttpStatusCode.InternalServerError, new { errors = new { body = new[] { "An unexpected error occurred" } } })
        };

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(errors);
        await httpContext.Response.WriteAsync(json, cancellationToken);

        return true;
    }
}
