using Application.DTOs;
using Core.Entities;

namespace Application.Interfaces;

public interface IOfferService
{
    Task<List<OfferDto>> GetOffersAsync();
    Task<List<OfferDto>> GetOffersAsync(int pageIndex, int pageSize);
    Task<OfferDto?> GetOfferByIdAsync(int id);
    Task<OfferDto?> UpdateOffer(int id, OfferDto updatedOffer);
    Task<bool> DeleteOfferItemAsync(int offerId, int itemId);
    Task<int> GetTotalOffersCountAsync();
    Task<IEnumerable<OfferItem>> GetOfferItemsNamesAsync();
    Task<OfferDto?> CreateNewOfferAsync();
}