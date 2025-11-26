using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Application.Validations;
using Application.Validations.Models;
using Core.Processors;

namespace Application.Services;

public class OfferImportService : IOfferImportService
{
    private readonly IOfferImportProcessor _processor;

    public OfferImportService(IOfferImportProcessor processor)
    {
        _processor = processor;
    }

    public async Task<ImportResultDto> ImportOffersAsync(
        List<OfferImportDto> offers,
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        var result = new ImportResultDto { ProcessedDate = DateTime.UtcNow };

        foreach (var batch in offers.Chunk(batchSize))
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var offer in batch)
                try
                {
                    var validationError = OfferValidator.ValidateOfferRow(offer);
                    if (validationError != null)
                    {
                        validationError.RowNumber = offer.RowNumber; // Set row number
                        result.Errors.Add(validationError);
                        continue;
                    }

                    var request = OfferMapper.ToProcessingRequest(offer);
                    var operationResult  = await _processor.ProcessOfferAsync(request);

                    if (operationResult.Success)
                        result.SuccessCount++;
                    else
                        result.Errors.Add(new RowError
                        {
                            RowNumber = offer.RowNumber,
                            Errors = operationResult.Errors
                        });
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new RowError
                    {
                        RowNumber = offer.RowNumber,
                        Errors = { ex.Message }
                    });
                }

            await Task.Delay(100, cancellationToken);
        }

        return result;
    }
}