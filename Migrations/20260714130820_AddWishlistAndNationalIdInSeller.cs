using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddWishlistAndNationalIdInSeller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NationalId",
                table: "Sellers",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "wishlists",
                columns: table => new
                {
                    WishlistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WishlistName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BuyerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wishlists", x => x.WishlistId);
                    table.ForeignKey(
                        name: "FK_wishlists_Buyers_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "Buyers",
                        principalColumn: "BuyerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wishlistItems",
                columns: table => new
                {
                    WishlistItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WishlistId = table.Column<int>(type: "int", nullable: false),
                    productVariantId = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wishlistItems", x => x.WishlistItemId);
                    table.ForeignKey(
                        name: "FK_wishlistItems_ProductVariants_productVariantId",
                        column: x => x.productVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "ProductVariantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_wishlistItems_wishlists_WishlistId",
                        column: x => x.WishlistId,
                        principalTable: "wishlists",
                        principalColumn: "WishlistId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_NationalId",
                table: "Sellers",
                column: "NationalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wishlistItems_productVariantId",
                table: "wishlistItems",
                column: "productVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_wishlistItems_WishlistId",
                table: "wishlistItems",
                column: "WishlistId");

            migrationBuilder.CreateIndex(
                name: "IX_wishlists_BuyerId",
                table: "wishlists",
                column: "BuyerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wishlistItems");

            migrationBuilder.DropTable(
                name: "wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Sellers_NationalId",
                table: "Sellers");

            migrationBuilder.DropColumn(
                name: "NationalId",
                table: "Sellers");
        }
    }
}
