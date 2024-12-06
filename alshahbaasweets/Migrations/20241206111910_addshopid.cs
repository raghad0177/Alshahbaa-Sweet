using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alshahbaasweets.Migrations
{
    /// <inheritdoc />
    public partial class addshopid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShopId",
                table: "Order_Item",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_Item_ShopId",
                table: "Order_Item",
                column: "ShopId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Item_Shop_ShopId",
                table: "Order_Item",
                column: "ShopId",
                principalTable: "Shop",
                principalColumn: "shop_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Item_Shop_ShopId",
                table: "Order_Item");

            migrationBuilder.DropIndex(
                name: "IX_Order_Item_ShopId",
                table: "Order_Item");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "Order_Item");
        }
    }
}
