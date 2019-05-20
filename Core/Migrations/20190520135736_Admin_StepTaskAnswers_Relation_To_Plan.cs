using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class Admin_StepTaskAnswers_Relation_To_Plan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlanId",
                table: "StepTaskAnswers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StepTaskAnswers_PlanId",
                table: "StepTaskAnswers",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_StepTaskAnswers_Plans_PlanId",
                table: "StepTaskAnswers",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StepTaskAnswers_Plans_PlanId",
                table: "StepTaskAnswers");

            migrationBuilder.DropIndex(
                name: "IX_StepTaskAnswers_PlanId",
                table: "StepTaskAnswers");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "StepTaskAnswers");
        }
    }
}
