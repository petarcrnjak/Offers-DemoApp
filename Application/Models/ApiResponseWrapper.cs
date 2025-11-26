namespace Application.Models;

public class ApiResponseWrapper<T>
{
    public ApiResponseWrapper(T? data, string message = "", bool success = true)
    {
        Success = success;
        Message = message;
        Timestamp = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss");
        Data = data;
    }

    public bool Success { get; set; }
    public string Message { get; set; }

    public string Timestamp { get; set; }
    public T? Data { get; set; }
}