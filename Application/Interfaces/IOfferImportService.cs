using Application.DTOs;

namespace Application.Interfaces;

public interface IOfferImportService
{
    Task<ImportResultDto> ImportOffersAsync(List<OfferImportDto> offers, int batchSize, CancellationToken cancellationToken = default);
}