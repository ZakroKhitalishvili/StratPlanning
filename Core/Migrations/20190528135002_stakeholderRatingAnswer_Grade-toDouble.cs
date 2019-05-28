using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class stakeholderRatingAnswer_GradetoDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Grade",
                table: "StakeholderRatingAnswers",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Grade",
                table: "StakeholderRatingAnswers",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);
        }
    }
}
