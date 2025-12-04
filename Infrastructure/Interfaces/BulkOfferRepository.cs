using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Interfaces;

public class BulkOfferRepository : IBulkOfferRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    private readonly IOfferItemRepository _items;
    private readonly IDbExceptionParser _dbExceptionParser;

    public BulkOfferRepository(IDbContextFactory<ApplicationDbContext> factory, IOfferItemRepository items, IDbExceptionParser dbExceptionParser)
    {
        _factory = factory;
        _items = items;
        _dbExceptionParser = dbExceptionParser;
    }

    public Task<OfferItem?> GetOfferItemByIdAsync(int id) => _items.GetByIdAsync(id);

    public Task<OfferItem> CreateNewOfferItem(OfferItem offerItem, CancellationToken ct = default)
       => _items.CreateAsync(offerItem, ct);

    public async Task<bool> AddOfferDetailAsync(int offerId, int offerItemId, int quantity, CancellationToken cancellationToken = default)
    {
        await using var context = _factory.CreateDbContext();

        var detail = new OfferDetails
        {
            OfferId = offerId,
            OfferItemId = offerItemId,
            Quantity = quantity
        };

        context.OfferDetails.Add(detail);

        try
        {
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (DbUpdateException ex) when (_dbExceptionParser.IsUniqueKeyViolation(ex, "IX_OfferDetails_OfferId_OfferItemId"))
        {
            // Duplicate (OfferId, OfferItemId)
            return false;
        }
        catch (DbUpdateException)
        {
            // Could be FK violation (offer or item missing) or other DB errors.
            // Keep behavior simple: signal not-added.
            return false;
        }
    }

    public async Task<Offer> CreateNewOfferImportAsync(Offer offer, CancellationToken cancellationToken = default)
    {
        await using var context = _factory.CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Enable IDENTITY_INSERT
            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Offers ON", cancellationToken);

            // Add and save the offer
            context.Offers.Add(offer);
            await context.SaveChangesAsync(cancellationToken);

            // Disable IDENTITY_INSERT
            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Offers OFF", cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return offer;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
