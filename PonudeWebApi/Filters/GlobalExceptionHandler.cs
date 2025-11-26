using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PonudeWebApi.Filters;

public class GlobalExceptionHandler : IExceptionHandler
{
    private static IWebHostEnvironment? _env;
    private static string? _traceId;

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        context.Response.ContentType = "application/json";

        _env ??= context.RequestServices.GetRequiredService<IWebHostEnvironment>();

        _traceId = context.TraceIdentifier;
        if (exception is ValidationException validationException)
        {
            await HandleValidationExceptionAsync(context, validationException, cancellationToken);
            return true;
        }

        await HandleGenericExceptionAsync(context, exception, cancellationToken);
        return true; // Exception is handled
    }

    private static async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var problemDetails = new ValidationProblemDetails
        {
            Type = "https://httpstatuses.com/400",
            Title = "Validation Failed",
            Status = StatusCodes.Status400BadRequest,
            Detail = "One or more validation errors occurred.",
            Instance = context.Request.Path
        };
        AddTraceIdStackTrace(problemDetails, exception);

        // Add validation errors
        foreach (var error in exception.Errors)
            problemDetails.Errors.Add(error.PropertyName, new[] { error.ErrorMessage });

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }

    private static async Task HandleGenericExceptionAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Type = "https://httpstatuses.com/500",
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.Message,
            Instance = context.Request.Path
        };
        AddTraceIdStackTrace(problemDetails, exception);

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }

    private static T AddTraceIdStackTrace<T>(T problemDetails, Exception exception) where T : ProblemDetails
    {
        problemDetails.Extensions["traceId"] = _traceId;

        if (_env != null && _env.IsDevelopment())
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;

        return problemDetails;
    }
}