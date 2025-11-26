using Application.DTOs;
using Application.Models;

namespace Application.Interfaces;

public interface IOfferItemApiClient
{
    Task<PaginatedOffersDto?> GetPaginatedOffersAsync(int pageIndex);
    Task<OfferDto?> GetOfferByIdAsync(int id);
    Task<ApiResponse<OfferDto>> UpdateOfferItem(OfferDto offer);
    Task<bool> DeleteOfferItem(int offerId, int itemId);
    Task<OfferDto?> CreateNewOfferAsync();
    Task<List<OfferItemDto>?> GetOfferItemsNames();
    Task<ImportResultDto?> ImportOffersAsync(List<OfferImportDto> offers);
}