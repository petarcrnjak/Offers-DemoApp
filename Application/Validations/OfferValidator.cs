using Application.DTOs;
using Application.Validations.Models;

namespace Application.Validations;

public static class OfferValidator
{
    public static RowError? ValidateOfferRow(OfferImportDto offerRow)
    {
        var rowError = new RowError();

        if (offerRow.Quantity <= 0)
            rowError.Errors.Add("Količina mora biti > 0");

        if (offerRow.UnitPrice <= 0)
            rowError.Errors.Add("Cijena mora biti > 0");

        if (offerRow.UnitPrice == null)
            rowError.Errors.Add("Cijena ne može biti null");

        if (string.IsNullOrWhiteSpace(offerRow.Article))
            rowError.Errors.Add("Naziv artikla je obavezan");

        return rowError.Errors.Count > 0 ? rowError : null;
    }
}