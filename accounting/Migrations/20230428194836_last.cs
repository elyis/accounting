using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace accounting.Migrations
{
    public partial class last : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialProduct");

            migrationBuilder.CreateTable(
                name: "ConsumptionOfMaterialPerProduct",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumptionOfMaterialPerProduct", x => new { x.MaterialId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_ConsumptionOfMaterialPerProduct_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsumptionOfMaterialPerProduct_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsumptionOfMaterialPerProduct_ProductId",
                table: "ConsumptionOfMaterialPerProduct",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsumptionOfMaterialPerProduct");

            migrationBuilder.CreateTable(
                name: "MaterialProduct",
                columns: table => new
                {
                    MaterialsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialProduct", x => new { x.MaterialsId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_MaterialProduct_Materials_MaterialsId",
                        column: x => x.MaterialsId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialProduct_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialProduct_ProductsId",
                table: "MaterialProduct",
                column: "ProductsId");
        }
    }
}
