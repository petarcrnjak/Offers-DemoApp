using Core.Entities;

namespace Core.Interfaces;

public interface IOfferItemRepository
{
    Task<OfferItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OfferItem> CreateAsync(OfferItem offerItem, CancellationToken cancellationToken = default);
    Task<IEnumerable<OfferItem>> GetOfferItemsNamesAsync(CancellationToken cancellationToken = default);
}
