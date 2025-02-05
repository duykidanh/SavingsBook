using System.Net;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SavingsBook.HostApi.Middleware;

public class GlobalMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalMiddleware> _logger;

    public GlobalMiddleware(RequestDelegate next, ILogger<GlobalMiddleware> logger)
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
            _logger.LogError(ex, "An error occurred while processing the request.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "Internal Server Error.",
            Details = exception.Message
        };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}