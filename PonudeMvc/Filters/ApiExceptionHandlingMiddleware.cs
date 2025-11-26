namespace Ponude.Filters;

public class ApiExceptionHandlingMiddleware
{
    private readonly ILogger<ApiExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ApiExceptionHandlingMiddleware(RequestDelegate next, ILogger<ApiExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Continue processing the request
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"API call failed: {ex.Message}");
            await context.Response.WriteAsync("The external API is currently unavailable."); // Throw custom exception
        }
        catch (Exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("An unexpected error occurred.");
        }
    }
}

public class ApiException : Exception
{
    public ApiException(string message) : base(message)
    {
    }
}