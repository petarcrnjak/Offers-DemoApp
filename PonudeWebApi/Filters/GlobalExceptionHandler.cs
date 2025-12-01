using Application.Models;
using Application.Models.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        // Add validation errors
        var errors = exception.Errors
            .Select(e => new ValidationError(e.PropertyName ?? string.Empty, e.ErrorMessage))
            .ToList();

        var apiResponse = new ApiResponseWrapper<List<ValidationError>>(errors, "Validation Failed", false);

        await context.Response.WriteAsJsonAsync(apiResponse, cancellationToken);
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