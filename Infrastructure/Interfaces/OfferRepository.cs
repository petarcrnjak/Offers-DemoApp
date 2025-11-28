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

    public async Task<Offer?> UpdateOffer(Offer updatedOffer)
    {
        _context.Offers.Update(updatedOffer);
        var result = await _context.SaveChangesAsync();

        return result > 0 ? updatedOffer : null;
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

    public async Task<Offer> CreateNewOfferImportAsync(Offer offer)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Enable IDENTITY_INSERT
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Offers ON");

            // Add and save the offer
            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();

            // Disable IDENTITY_INSERT
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Offers OFF");

            await transaction.CommitAsync();
            return offer;
        }
        catch
        {
            await transaction.RollbackAsync();
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

    public async Task<OfferItem> CreateNewOfferItem(OfferItem offerItem)
    {
        _context.OfferItems.Add(offerItem);
        await _context.SaveChangesAsync();

        return offerItem;
    }
}