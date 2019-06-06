using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class StepTask_IsActive_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "StepTasks",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "StepTasks");
        }
    }
}
