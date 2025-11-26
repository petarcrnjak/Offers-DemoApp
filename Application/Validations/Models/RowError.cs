using System.Text.Json.Serialization;

namespace Application.Validations.Models;

public class RowError
{
    public int RowNumber { get; set; }
    public List<string> Errors { get; set; } = new();

    [JsonIgnore] public bool HasErrors => Errors.Count != 0;
}