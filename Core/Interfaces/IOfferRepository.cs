using Core.Entities;

namespace Core.Interfaces;

public interface IOfferRepository
{
    Task<IEnumerable<Offer>> GetAllOffersAsync();
    Task<IEnumerable<Offer>> GetAllOffersAsync(int pageIndex, int pageSize);
    Task<Offer?> GetOfferByIdAsync(int id);
    Task<OfferItem?> GetOfferItemByIdAsync(int id);
    Task<Offer?> UpdateOfferAsync(Offer updatedOffer, CancellationToken cancellationToken = default);
    Task<bool> DeleteOfferItemAsync(int offerId, int itemId);
    Task<int> GetTotalOffersCountAsync();
    Task<Offer> CreateNewOfferAsync(Offer offer);
    Task<bool> OfferItemExistsAsync(int offerDtoOfferId, int articleId);
}