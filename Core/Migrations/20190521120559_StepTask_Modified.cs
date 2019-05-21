using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class StepTask_Modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserToPlanId",
                table: "StepTaskAnswers",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserToPlanId",
                table: "StepTaskAnswers",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
