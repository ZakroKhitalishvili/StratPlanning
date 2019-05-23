using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class Dictionary_hasValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_StepTaskAnswers_Plans_PlanId",
            //    table: "StepTaskAnswers");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_StepTaskAnswers_UserStepResults_UserToPlanId",
            //    table: "StepTaskAnswers");

            //migrationBuilder.DropIndex(
            //    name: "IX_StepTaskAnswers_PlanId",
            //    table: "StepTaskAnswers");

            //migrationBuilder.DropColumn(
            //    name: "PlanId",
            //    table: "StepTaskAnswers");

            migrationBuilder.AlterColumn<bool>(
                name: "HasPosition",
                table: "Dictionaries",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "HasCriterion",
                table: "Dictionaries",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<bool>(
                name: "HasValue",
                table: "Dictionaries",
                nullable: false,
                defaultValue: false);

            //migrationBuilder.CreateIndex(
            //    name: "IX_StepTaskAnswers_UserStepResultId",
            //    table: "StepTaskAnswers",
            //    column: "UserStepResultId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_StepTaskAnswers_UserStepResults_UserStepResultId",
            //    table: "StepTaskAnswers",
            //    column: "UserStepResultId",
            //    principalTable: "UserStepResults",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_StepTaskAnswers_UserStepResults_UserStepResultId",
            //    table: "StepTaskAnswers");

            //migrationBuilder.DropIndex(
            //    name: "IX_StepTaskAnswers_UserStepResultId",
            //    table: "StepTaskAnswers");

            migrationBuilder.DropColumn(
                name: "HasValue",
                table: "Dictionaries");

            //migrationBuilder.AddColumn<int>(
            //    name: "PlanId",
            //    table: "StepTaskAnswers",
            //    nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HasPosition",
                table: "Dictionaries",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "HasCriterion",
                table: "Dictionaries",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            //migrationBuilder.CreateIndex(
            //    name: "IX_StepTaskAnswers_PlanId",
            //    table: "StepTaskAnswers",
            //    column: "PlanId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_StepTaskAnswers_Plans_PlanId",
            //    table: "StepTaskAnswers",
            //    column: "PlanId",
            //    principalTable: "Plans",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_StepTaskAnswers_UserStepResults_UserToPlanId",
            //    table: "StepTaskAnswers",
            //    column: "UserToPlanId",
            //    principalTable: "UserStepResults",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }
    }
}
