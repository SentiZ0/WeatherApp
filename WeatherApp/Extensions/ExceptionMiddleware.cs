using System.Net;

namespace WeatherApp.Extensions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError($"Bad request: {ex.Message}");
            await HandleExceptionAsync(httpContext, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError($"Resource not found: {ex.Message}");
            await HandleExceptionAsync(httpContext, HttpStatusCode.NotFound, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong: {ex.Message}");
            await HandleExceptionAsync(httpContext, HttpStatusCode.InternalServerError,
                "An unexpected error occurred.");
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        
        var result = System.Text.Json.JsonSerializer.Serialize(new 
        {
            error = message,
            statusCode = (int)statusCode
        });
        
        return context.Response.WriteAsync(result);
    }
}