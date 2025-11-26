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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OfferDetails>()
            .ToTable("OfferDetails"); // Explicit table name

        modelBuilder.Entity<OfferDetails>()
            .HasKey(oim => new { oim.OfferId, oim.OfferItemId }); // Composite key

        modelBuilder.Entity<OfferDetails>()
            .HasOne(oim => oim.Offer)
            .WithMany(o => o.OfferDetails)
            .HasForeignKey(oim => oim.OfferId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OfferDetails>()
            .HasOne(oim => oim.OfferItem)
            .WithMany(oi => oi.OfferDetails)
            .HasForeignKey(oim => oim.OfferItemId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}