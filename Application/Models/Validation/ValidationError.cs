namespace Application.Models.Validation;

public class ValidationError
{
    public ValidationError(string name, string errorMessage)
    {
        Name = name;
        ErrorMessage = errorMessage;
    }

    public string Name { get; set; }
    public string ErrorMessage { get; set; }
}