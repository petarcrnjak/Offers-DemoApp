using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Interfaces;

public class OfferRepository : IOfferRepository
{
    private readonly ApplicationDbContext _context;

    public OfferRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Offer>> GetAllOffersAsync()
    {
        return await _context.Offers
            .AsNoTracking()
            .Include(o => o.OfferDetails) // Include OfferItems if needed
            .ThenInclude(x => x.OfferItem)
            .ToListAsync();
    }

    public async Task<IEnumerable<Offer>> GetAllOffersAsync(int pageIndex, int pageSize)
    {
        return await _context.Offers
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Offer?> GetOfferByIdAsync(int id)
    {
        return await _context.Offers
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Include(o => o.OfferDetails) // Include OfferItems if needed
            .ThenInclude(x => x.OfferItem)
            .FirstOrDefaultAsync();
    }

    public async Task<OfferItem?> GetOfferItemByIdAsync(int id)
    {
        return await _context.OfferItems
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Offer?> UpdateOffer(Offer updatedOffer, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Offers
           .Include(o => o.OfferDetails)
           .ThenInclude(od => od.OfferItem)
           .FirstOrDefaultAsync(o => o.Id == updatedOffer.Id, cancellationToken);

        if (existing == null)
            return null;

        var updatedDetails = updatedOffer.OfferDetails ?? new List<OfferDetails>();
        var updatedItemIds = updatedDetails.Select(d => d.OfferItemId).ToHashSet();

        var toRemove = existing.OfferDetails
           .Where(d => !updatedItemIds.Contains(d.OfferItemId))
           .ToList();

        foreach (var rem in toRemove)
        {
            // remove from navigation and mark for deletion
            existing.OfferDetails.Remove(rem);
            _context.Remove(rem);
        }

        // Update existing details and add new ones
        foreach (var upd in updatedDetails)
        {
            var existingDetail = existing.OfferDetails.FirstOrDefault(d => d.OfferItemId == upd.OfferItemId);
            if (existingDetail != null)
            {
                if (existingDetail.Quantity != upd.Quantity)
                    existingDetail.Quantity = upd.Quantity;
            }
            else
            {
                // Ensure OfferItem is attached/tracked
                var item = await _context.OfferItems.FindAsync(upd.OfferItemId);
                if (item == null)
                {
                    // Skip relations for missing OfferItem - caller/service should ensure items exist
                    continue;
                }

                var newDetail = new OfferDetails
                {
                    OfferId = existing.Id,
                    OfferItemId = item.Id,
                    OfferItem = item,
                    Quantity = upd.Quantity
                };

                existing.OfferDetails.Add(newDetail);
            }
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return existing;
        }
        catch (DbUpdateConcurrencyException)
        {
            return null;
        }
    }

    public async Task<bool> DeleteOfferItemAsync(int offerId, int itemId)
    {
        var offer = await _context.Offers
            .Where(o => o.Id == offerId)
            .Include(o => o.OfferDetails) // Include OfferItems if needed
            .ThenInclude(x => x.OfferItem)
            .FirstOrDefaultAsync();

        if (offer == null)
            return false;

        var itemToRemove = offer.OfferDetails.FirstOrDefault(x => x.OfferId == offerId && x.OfferItemId == itemId);
        if (itemToRemove == null)
            return false;

        offer.OfferDetails.Remove(itemToRemove);
        _context.Remove(itemToRemove);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetTotalOffersCount()
    {
        return await _context.Offers
            .AsNoTracking()
            .CountAsync();
    }

    public async Task<IEnumerable<OfferItem>> GetOfferItemsNames()
    {
        return await _context.OfferItems
            .AsNoTracking()
            .OrderBy(x => x.Article)
            .ToListAsync();
    }

    public async Task<Offer> CreateNewOfferImportAsync(Offer offer, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Enable IDENTITY_INSERT
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Offers ON", cancellationToken);

            // Add and save the offer
            _context.Offers.Add(offer);
            await _context.SaveChangesAsync(cancellationToken);

            // Disable IDENTITY_INSERT
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Offers OFF", cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return offer;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Offer> CreateNewOfferAsync(Offer offer)
    {
        _context.Offers.Add(offer);
        await _context.SaveChangesAsync();

        return offer;
    }

    public async Task<bool> OfferItemExistsAsync(int offerId, int articleId)
    {
        return await _context.Offers
            .AsNoTracking()
            .Where(o => o.Id == offerId)
            .SelectMany(o => o.OfferDetails)
            .AnyAsync(od => od.OfferItemId == articleId);
    }

    public async Task<OfferItem> CreateNewOfferItem(OfferItem offerItem, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.OfferItems ON", cancellationToken);

            _context.OfferItems.Add(offerItem);
            await _context.SaveChangesAsync(cancellationToken);

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.OfferItems OFF", cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return offerItem;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}