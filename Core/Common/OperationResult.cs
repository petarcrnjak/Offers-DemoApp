namespace Core.Common;

public class OperationResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();

    public static OperationResult Ok() => new() { Success = true };

    public static OperationResult Fail(params string[] errors) => new() { Success = false, Errors = errors.ToList() };
}