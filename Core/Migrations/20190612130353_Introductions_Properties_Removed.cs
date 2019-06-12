using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class Introductions_Properties_Removed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Introductions");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Introductions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Introductions",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Introductions",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
