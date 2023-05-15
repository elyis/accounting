using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace accounting.Migrations
{
    public partial class last2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsumptionOfMaterialPerProduct_Materials_MaterialId",
                table: "ConsumptionOfMaterialPerProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsumptionOfMaterialPerProduct_Products_ProductId",
                table: "ConsumptionOfMaterialPerProduct");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsumptionOfMaterialPerProduct_Materials_MaterialId",
                table: "ConsumptionOfMaterialPerProduct",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsumptionOfMaterialPerProduct_Products_ProductId",
                table: "ConsumptionOfMaterialPerProduct",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsumptionOfMaterialPerProduct_Materials_MaterialId",
                table: "ConsumptionOfMaterialPerProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsumptionOfMaterialPerProduct_Products_ProductId",
                table: "ConsumptionOfMaterialPerProduct");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsumptionOfMaterialPerProduct_Materials_MaterialId",
                table: "ConsumptionOfMaterialPerProduct",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsumptionOfMaterialPerProduct_Products_ProductId",
                table: "ConsumptionOfMaterialPerProduct",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
