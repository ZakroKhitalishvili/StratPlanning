using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class StepTaskAnswers_Attached_To_UserStepResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "StepTaskAnswers",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "StepTaskAnswers",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "StepTaskAnswers",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "UserStepResultId",
                table: "StepTaskAnswers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_StepTaskAnswers_UserStepResults_UserStepResultId",
                table: "StepTaskAnswers",
                column: "UserStepResultId",
                principalTable: "UserStepResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StepTaskAnswers_UserStepResults_UserStepResultId",
                table: "StepTaskAnswers");

            migrationBuilder.DropColumn(
                name: "UserStepResultId",
                table: "StepTaskAnswers");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "StepTaskAnswers",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "StepTaskAnswers",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "StepTaskAnswers",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
