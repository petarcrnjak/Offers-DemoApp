namespace Ponude.Filters;

public class GlobalExceptionHandlingMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Continue with the request pipeline
        }
        catch (ApiException ex)
        {
            _logger.LogError($"API Exception: {ex.Message}");

            // Set up a fallback response or redirect to a page with a user-friendly message
            context.Response.StatusCode = 503; // Service Unavailable
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync("<h1>External API is currently unavailable. Please try again later.</h1>");
        }
    }
}