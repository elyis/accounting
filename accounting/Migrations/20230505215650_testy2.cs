using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace accounting.Migrations
{
    public partial class testy2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Urls",
                table: "MaterialAccountings",
                newName: "Images");

            migrationBuilder.CreateTable(
                name: "ProductAccountings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    CheckPrice = table.Column<float>(type: "real", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    Images = table.Column<List<string>>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAccountings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAccountings_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AmountOfAccountedProduct",
                columns: table => new
                {
                    ProductAccountingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmountOfAccountedProduct", x => new { x.ProductAccountingId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_AmountOfAccountedProduct_ProductAccountings_ProductAccounti~",
                        column: x => x.ProductAccountingId,
                        principalTable: "ProductAccountings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AmountOfAccountedProduct_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AmountOfAccountedProduct_ProductId",
                table: "AmountOfAccountedProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAccountings_CreatedById",
                table: "ProductAccountings",
                column: "CreatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AmountOfAccountedProduct");

            migrationBuilder.DropTable(
                name: "ProductAccountings");

            migrationBuilder.RenameColumn(
                name: "Images",
                table: "MaterialAccountings",
                newName: "Urls");
        }
    }
}
