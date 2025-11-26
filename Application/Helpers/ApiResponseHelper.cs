using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Application.Helpers;

public static class ApiResponseHelper
{
    public static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static ActionResult<T> NotFoundResponse<T>(string message) where T : class
    {
        message = string.IsNullOrWhiteSpace(message) ? "Not Found" : message;
        var response = new ApiResponseWrapper<T>(null, message, false);
        return new NotFoundObjectResult(response);
    }

    public static ActionResult BadRequestResponse(string message)
    {
        var response = new ApiResponseWrapper<object>(null, message, false);
        return new BadRequestObjectResult(response);
    }

    public static ApiResponseWrapper<T> CreateErrorResponse<T>(string message, T? data = default, bool success = false)
    {
        return new ApiResponseWrapper<T>(data, message, success);
    }
}