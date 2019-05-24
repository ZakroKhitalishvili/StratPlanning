using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class SWOTAnswers_ReAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueOptionAnswers_SWOTAnswer_IssueId",
                table: "IssueOptionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_SelectAnswers_SWOTAnswer_IssueId",
                table: "SelectAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StrategicIssueAnswers_SWOTAnswer_IssueId",
                table: "StrategicIssueAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_SWOTAnswer_Questions_QuestionId",
                table: "SWOTAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_SWOTAnswer_UserStepResults_UserStepResultId",
                table: "SWOTAnswer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SWOTAnswer",
                table: "SWOTAnswer");

            migrationBuilder.RenameTable(
                name: "SWOTAnswer",
                newName: "SWOTAnswers");

            migrationBuilder.RenameIndex(
                name: "IX_SWOTAnswer_UserStepResultId",
                table: "SWOTAnswers",
                newName: "IX_SWOTAnswers_UserStepResultId");

            migrationBuilder.RenameIndex(
                name: "IX_SWOTAnswer_QuestionId",
                table: "SWOTAnswers",
                newName: "IX_SWOTAnswers_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SWOTAnswers",
                table: "SWOTAnswers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueOptionAnswers_SWOTAnswers_IssueId",
                table: "IssueOptionAnswers",
                column: "IssueId",
                principalTable: "SWOTAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SelectAnswers_SWOTAnswers_IssueId",
                table: "SelectAnswers",
                column: "IssueId",
                principalTable: "SWOTAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StrategicIssueAnswers_SWOTAnswers_IssueId",
                table: "StrategicIssueAnswers",
                column: "IssueId",
                principalTable: "SWOTAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SWOTAnswers_Questions_QuestionId",
                table: "SWOTAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SWOTAnswers_UserStepResults_UserStepResultId",
                table: "SWOTAnswers",
                column: "UserStepResultId",
                principalTable: "UserStepResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueOptionAnswers_SWOTAnswers_IssueId",
                table: "IssueOptionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_SelectAnswers_SWOTAnswers_IssueId",
                table: "SelectAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StrategicIssueAnswers_SWOTAnswers_IssueId",
                table: "StrategicIssueAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_SWOTAnswers_Questions_QuestionId",
                table: "SWOTAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_SWOTAnswers_UserStepResults_UserStepResultId",
                table: "SWOTAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SWOTAnswers",
                table: "SWOTAnswers");

            migrationBuilder.RenameTable(
                name: "SWOTAnswers",
                newName: "SWOTAnswer");

            migrationBuilder.RenameIndex(
                name: "IX_SWOTAnswers_UserStepResultId",
                table: "SWOTAnswer",
                newName: "IX_SWOTAnswer_UserStepResultId");

            migrationBuilder.RenameIndex(
                name: "IX_SWOTAnswers_QuestionId",
                table: "SWOTAnswer",
                newName: "IX_SWOTAnswer_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SWOTAnswer",
                table: "SWOTAnswer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueOptionAnswers_SWOTAnswer_IssueId",
                table: "IssueOptionAnswers",
                column: "IssueId",
                principalTable: "SWOTAnswer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SelectAnswers_SWOTAnswer_IssueId",
                table: "SelectAnswers",
                column: "IssueId",
                principalTable: "SWOTAnswer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StrategicIssueAnswers_SWOTAnswer_IssueId",
                table: "StrategicIssueAnswers",
                column: "IssueId",
                principalTable: "SWOTAnswer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SWOTAnswer_Questions_QuestionId",
                table: "SWOTAnswer",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SWOTAnswer_UserStepResults_UserStepResultId",
                table: "SWOTAnswer",
                column: "UserStepResultId",
                principalTable: "UserStepResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
