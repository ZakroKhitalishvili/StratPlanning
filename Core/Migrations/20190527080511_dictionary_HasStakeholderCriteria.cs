using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class dictionary_HasStakeholderCriteria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasStakeholderCriteria",
                table: "Dictionaries",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasStakeholderCriteria",
                table: "Dictionaries");
        }
    }
}
