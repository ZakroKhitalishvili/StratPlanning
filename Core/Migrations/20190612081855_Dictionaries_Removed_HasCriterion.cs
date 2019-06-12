using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class Dictionaries_Removed_HasCriterion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasCriterion",
                table: "Dictionaries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasCriterion",
                table: "Dictionaries",
                nullable: false,
                defaultValue: false);
        }
    }
}
