using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class IssueOptionAnswers_PreparingAnswers_Relation_Modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PreparingAnswers_IssueOptionAnswerId",
                table: "PreparingAnswers");

            migrationBuilder.CreateIndex(
                name: "IX_PreparingAnswers_IssueOptionAnswerId",
                table: "PreparingAnswers",
                column: "IssueOptionAnswerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PreparingAnswers_IssueOptionAnswerId",
                table: "PreparingAnswers");

            migrationBuilder.CreateIndex(
                name: "IX_PreparingAnswers_IssueOptionAnswerId",
                table: "PreparingAnswers",
                column: "IssueOptionAnswerId",
                unique: true);
        }
    }
}
