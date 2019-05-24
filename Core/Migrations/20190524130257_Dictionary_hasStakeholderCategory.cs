using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class Dictionary_hasStakeholderCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasStakeholderCategory",
                table: "Dictionaries",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasStakeholderCategory",
                table: "Dictionaries");
        }
    }
}
