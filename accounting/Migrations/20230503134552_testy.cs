using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace accounting.Migrations
{
    public partial class testy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_MaterialAccountings_MaterialAccountingId",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_MaterialAccountingId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "MaterialAccountingId",
                table: "Materials");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MaterialAccountingId",
                table: "Materials",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materials_MaterialAccountingId",
                table: "Materials",
                column: "MaterialAccountingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_MaterialAccountings_MaterialAccountingId",
                table: "Materials",
                column: "MaterialAccountingId",
                principalTable: "MaterialAccountings",
                principalColumn: "Id");
        }
    }
}
