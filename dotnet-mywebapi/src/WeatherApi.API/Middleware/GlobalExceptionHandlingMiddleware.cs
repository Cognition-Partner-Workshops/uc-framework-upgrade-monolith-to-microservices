using System.Net;
using System.Text.Json;
using WeatherApi.Domain.Exceptions;

namespace WeatherApi.API.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, response) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = validationEx.Message,
                    Errors = validationEx.Errors
                }),
            NotFoundException => (
                HttpStatusCode.NotFound,
                new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = exception.Message
                }),
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = exception.Message
                }),
            InvalidOperationException => (
                HttpStatusCode.Conflict,
                new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Message = exception.Message
                }),
            ArgumentException => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = exception.Message
                }),
            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "An internal server error occurred."
                })
        };

        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public IDictionary<string, string[]>? Errors { get; set; }
}
