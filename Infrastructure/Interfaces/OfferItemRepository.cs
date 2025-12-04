using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Interfaces;

public class OfferItemRepository : IOfferItemRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public OfferItemRepository(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<OfferItem?> GetByIdAsync(int id, CancellationToken cancelationToken = default)
    {
        await using var context = _factory.CreateDbContext();
        return await context.OfferItems.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancelationToken);
    }

    public async Task<OfferItem> CreateAsync(OfferItem offerItem, CancellationToken cancelationToken = default)
    {
        await using var context = _factory.CreateDbContext();
        context.OfferItems.Add(offerItem);
        await context.SaveChangesAsync(cancelationToken);

        return offerItem;
    }

    public async Task<IEnumerable<OfferItem>> GetOfferItemsNamesAsync(CancellationToken cancellationToken = default)
    {
        await using var context = _factory.CreateDbContext();
        return await context.OfferItems
            .AsNoTracking()
            .OrderBy(x => x.Article)
            .ToListAsync(cancellationToken);
    }
}
