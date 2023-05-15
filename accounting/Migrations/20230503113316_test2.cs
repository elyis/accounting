using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace accounting.Migrations
{
    public partial class test2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialMaterialAccounting");

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialAccountingId",
                table: "Materials",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AmountOfAccountedMaterials",
                columns: table => new
                {
                    MaterialAccountingId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmountOfAccountedMaterials", x => new { x.MaterialAccountingId, x.MaterialId });
                    table.ForeignKey(
                        name: "FK_AmountOfAccountedMaterials_MaterialAccountings_MaterialAcco~",
                        column: x => x.MaterialAccountingId,
                        principalTable: "MaterialAccountings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AmountOfAccountedMaterials_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Materials_MaterialAccountingId",
                table: "Materials",
                column: "MaterialAccountingId");

            migrationBuilder.CreateIndex(
                name: "IX_AmountOfAccountedMaterials_MaterialId",
                table: "AmountOfAccountedMaterials",
                column: "MaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_MaterialAccountings_MaterialAccountingId",
                table: "Materials",
                column: "MaterialAccountingId",
                principalTable: "MaterialAccountings",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_MaterialAccountings_MaterialAccountingId",
                table: "Materials");

            migrationBuilder.DropTable(
                name: "AmountOfAccountedMaterials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_MaterialAccountingId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "MaterialAccountingId",
                table: "Materials");

            migrationBuilder.CreateTable(
                name: "MaterialMaterialAccounting",
                columns: table => new
                {
                    MaterialAccountingsId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialMaterialAccounting", x => new { x.MaterialAccountingsId, x.MaterialsId });
                    table.ForeignKey(
                        name: "FK_MaterialMaterialAccounting_MaterialAccountings_MaterialAcco~",
                        column: x => x.MaterialAccountingsId,
                        principalTable: "MaterialAccountings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialMaterialAccounting_Materials_MaterialsId",
                        column: x => x.MaterialsId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaterialAccounting_MaterialsId",
                table: "MaterialMaterialAccounting",
                column: "MaterialsId");
        }
    }
}
