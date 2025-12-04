using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Offer> Offers { get; set; }
    public DbSet<OfferItem> OfferItems { get; set; }
    public DbSet<OfferDetails> OfferDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Offer>(x =>
        {
            x.HasKey(o => o.Id);
            x.Property(o => o.Date).IsRequired();
            x.HasMany(o => o.OfferDetails)
             .WithOne(od => od.Offer)
             .HasForeignKey(od => od.OfferId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // OfferItem -> OfferDetails (one-to-many)
        modelBuilder.Entity<OfferItem>(x =>
        {
            x.HasKey(ai => ai.Id);
            x.Property(ai => ai.Article).HasMaxLength(200);
            x.HasMany(ai => ai.OfferDetails)
             .WithOne(od => od.OfferItem)
             .HasForeignKey(od => od.OfferItemId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        //Index to enforce uniqueness of OfferId and OfferItemId combination
        modelBuilder.Entity<OfferDetails>(x =>
        {
            x.HasKey(od => new { od.OfferId, od.OfferItemId });

            // If you prefer an explicit unique index (name kept for legacy code that inspects message):
            x.HasIndex(od => new { od.OfferId, od.OfferItemId })
             .IsUnique()
             .HasDatabaseName("IX_OfferDetails_OfferId_OfferItemId");

            x.Property(od => od.Quantity).IsRequired();
        });
    }
}