using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class Dictionaries_IsDeleted_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Dictionaries",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Dictionaries");
        }
    }
}
