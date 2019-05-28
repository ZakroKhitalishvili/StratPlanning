using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class Questions_Modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StepBlockId",
                table: "Questions",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StepBlockId",
                table: "Questions",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
