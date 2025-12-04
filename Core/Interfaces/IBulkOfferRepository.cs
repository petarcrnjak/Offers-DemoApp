using Core.Entities;

namespace Core.Interfaces;

public interface IBulkOfferRepository
{
    Task<OfferItem?> GetOfferItemByIdAsync(int id);
    Task<Offer> CreateNewOfferImportAsync(Offer offer, CancellationToken cancellationToken = default);
    Task<bool> AddOfferDetailAsync(int offerId, int id, int quantity, CancellationToken cancellationToken = default);
    Task<OfferItem> CreateNewOfferItem(OfferItem offerItem, CancellationToken cancellationToken = default);
}
