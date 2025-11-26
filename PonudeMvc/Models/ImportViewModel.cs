using System.ComponentModel.DataAnnotations;
using Application.Validations.Models;

namespace Ponude.Models;

public class ImportViewModel
{
    [Required]
    [Display(Name = "Excel datoteka")]
    public IFormFile ExcelFile { get; set; }

    public ImportResult? ImportResult { get; set; }
}