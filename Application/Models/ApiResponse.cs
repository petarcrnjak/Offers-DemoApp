namespace Application.Models;

public class ApiResponse<T>
{
    public ApiResponse(bool success, string? errorMessage = null, T? data = default)
    {
        Success = success;
        Data = data;
        ErrorMessage = errorMessage;
    }

    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
}