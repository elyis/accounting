using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace accounting.Migrations
{
    public partial class isEnd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "MinimumQuantity",
                table: "Materials",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumQuantity",
                table: "Materials");
        }
    }
}
