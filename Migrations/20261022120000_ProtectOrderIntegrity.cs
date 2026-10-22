using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetFoundations.Migrations
{
    public partial class ProtectOrderIntegrity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_Orders_Clients_ClientId", "Orders");
            migrationBuilder.DropForeignKey("FK_Items_Products_ProductId", "Items");
            migrationBuilder.DropForeignKey("FK_Items_Orders_OrdersId", "Items");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Clients_ClientId",
                table: "Orders",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientsId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Products_ProductId",
                table: "Items",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductsId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Orders_OrdersId",
                table: "Items",
                column: "OrdersId",
                principalTable: "Orders",
                principalColumn: "OrdersId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_Orders_Clients_ClientId", "Orders");
            migrationBuilder.DropForeignKey("FK_Items_Products_ProductId", "Items");
            migrationBuilder.DropForeignKey("FK_Items_Orders_OrdersId", "Items");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Clients_ClientId",
                table: "Orders",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientsId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Products_ProductId",
                table: "Items",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductsId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Orders_OrdersId",
                table: "Items",
                column: "OrdersId",
                principalTable: "Orders",
                principalColumn: "OrdersId");
        }
    }
}
