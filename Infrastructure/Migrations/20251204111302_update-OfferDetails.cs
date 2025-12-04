using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateOfferDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfferDetails_OfferItems_OfferItemId",
                table: "OfferDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Article",
                table: "OfferItems",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_OfferDetails_OfferItems_OfferItemId",
                table: "OfferDetails",
                column: "OfferItemId",
                principalTable: "OfferItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfferDetails_OfferItems_OfferItemId",
                table: "OfferDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Article",
                table: "OfferItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferDetails_OfferItems_OfferItemId",
                table: "OfferDetails",
                column: "OfferItemId",
                principalTable: "OfferItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
