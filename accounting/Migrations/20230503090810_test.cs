﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace accounting.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaterialAccountings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialAccountings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialAccountings_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_MaterialAccountings_CreatedById",
                table: "MaterialAccountings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaterialAccounting_MaterialsId",
                table: "MaterialMaterialAccounting",
                column: "MaterialsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialMaterialAccounting");

            migrationBuilder.DropTable(
                name: "MaterialAccountings");
        }
    }
}
