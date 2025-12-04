using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_index_to_enable_uniqueness_on_offerDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OfferDetails_OfferId_OfferItemId",
                table: "OfferDetails",
                columns: new[] { "OfferId", "OfferItemId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OfferDetails_OfferId_OfferItemId",
                table: "OfferDetails");
        }
    }
}
