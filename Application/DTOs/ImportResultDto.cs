using Application.Validations.Models;

namespace Application.DTOs;

public class ImportResultDto
{
    public DateTime ProcessedDate { get; set; }
    public int SuccessCount { get; set; }
    public List<RowError> Errors { get; set; } = [];
}