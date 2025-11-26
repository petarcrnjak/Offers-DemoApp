using Application.DTOs;
using FluentValidation;

namespace Application.Validations;

public class OfferItemValidator : AbstractValidator<OfferItemDto>
{
    public OfferItemValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.");
    }
}