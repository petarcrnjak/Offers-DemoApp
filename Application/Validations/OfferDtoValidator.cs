using Application.DTOs;
using FluentValidation;

namespace Application.Validations;

public class OfferDtoValidator : AbstractValidator<OfferDto>
{
    public OfferDtoValidator(IValidator<OfferItemDto> itemValidator)
    {
        RuleFor(x => x).CustomAsync(async (offer, context, ct) =>
        {
            if (offer?.OfferItems == null)
                return;

            var tasks = offer.OfferItems.Select(i => itemValidator.ValidateAsync(i, ct)).ToArray();
            var results = await Task.WhenAll(tasks);

            for (int i = 0; i < results.Length; i++)
            {
                var result = results[i];
                if (!result.IsValid)
                {
                    var item = offer.OfferItems[i];
                    var prefix = string.IsNullOrWhiteSpace(item.Article) ? "Item" : item.Article;
                    foreach (var failure in result.Errors)
                    {
                        context.AddFailure(failure.PropertyName, $"{prefix}: {failure.ErrorMessage}");
                    }
                }
            }
        });
    }
}
