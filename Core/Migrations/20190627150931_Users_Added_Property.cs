using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class Users_Added_Property : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailBackUp",
                table: "Users",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailBackUp",
                table: "Users");
        }
    }
}
