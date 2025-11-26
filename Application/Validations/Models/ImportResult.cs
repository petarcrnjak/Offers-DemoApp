namespace Application.Validations.Models;

public class ImportResult
{
    public DateTime ProcessedDate { get; set; }
    public int SuccessCount { get; set; }
    public List<RowError> Errors { get; set; } = new();
}