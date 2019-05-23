using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class StakeholderAnswers_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StakeholderRatingAnswers_TextAnswers_StakeholderId",
                table: "StakeholderRatingAnswers");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_StepTaskAnswers_Plans_PlanId",
            //    table: "StepTaskAnswers");

            //migrationBuilder.DropIndex(
            //    name: "IX_StepTaskAnswers_PlanId",
            //    table: "StepTaskAnswers");

            migrationBuilder.DropColumn(
                name: "IsStakeholder",
                table: "TextAnswers");

            //migrationBuilder.DropColumn(
            //    name: "PlanId",
            //    table: "StepTaskAnswers");

            //migrationBuilder.AlterColumn<string>(
            //    name: "LastName",
            //    table: "StepTaskAnswers",
            //    maxLength: 50,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldMaxLength: 50);

            //migrationBuilder.AlterColumn<string>(
            //    name: "FirstName",
            //    table: "StepTaskAnswers",
            //    maxLength: 50,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldMaxLength: 50);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Email",
            //    table: "StepTaskAnswers",
            //    maxLength: 50,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldMaxLength: 50);

            //migrationBuilder.AddColumn<int>(
            //    name: "UserStepResultId",
            //    table: "StepTaskAnswers",
            //    nullable: false,
            //    defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StakeholderAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuestionId = table.Column<int>(nullable: false),
                    UserStepResultId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 50, nullable: true),
                    LastName = table.Column<string>(maxLength: 50, nullable: true),
                    Email = table.Column<string>(maxLength: 50, nullable: true),
                    IsInternal = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StakeholderAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StakeholderAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StakeholderAnswers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StakeholderAnswers_UserStepResults_UserStepResultId",
                        column: x => x.UserStepResultId,
                        principalTable: "UserStepResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            //migrationBuilder.CreateTable(
            //    name: "StepResponsibles",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Step = table.Column<string>(maxLength: 50, nullable: false),
            //        UserToPlanId = table.Column<int>(nullable: false),
            //        CreatedAt = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<int>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_StepResponsibles", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_StepResponsibles_UsersToPlans_UserToPlanId",
            //            column: x => x.UserToPlanId,
            //            principalTable: "UsersToPlans",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_StepTaskAnswers_UserStepResultId",
            //    table: "StepTaskAnswers",
            //    column: "UserStepResultId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_StakeholderAnswers_QuestionId",
            //    table: "StakeholderAnswers",
            //    column: "QuestionId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_StakeholderAnswers_UserId",
            //    table: "StakeholderAnswers",
            //    column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StakeholderAnswers_UserStepResultId",
                table: "StakeholderAnswers",
                column: "UserStepResultId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_StepResponsibles_UserToPlanId",
            //    table: "StepResponsibles",
            //    column: "UserToPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_StakeholderRatingAnswers_StakeholderAnswers_StakeholderId",
                table: "StakeholderRatingAnswers",
                column: "StakeholderId",
                principalTable: "StakeholderAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
            migrationBuilder.DropForeignKey(
                name: "FK_StakeholderRatingAnswers_StakeholderAnswers_StakeholderId",
                table: "StakeholderRatingAnswers");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_StepTaskAnswers_UserStepResults_UserStepResultId",
            //    table: "StepTaskAnswers");

            migrationBuilder.DropTable(
                name: "StakeholderAnswers");

            //migrationBuilder.DropTable(
            //    name: "StepResponsibles");

            //migrationBuilder.DropIndex(
            //    name: "IX_StepTaskAnswers_UserStepResultId",
            //    table: "StepTaskAnswers");

            //migrationBuilder.DropColumn(
            //    name: "UserStepResultId",
            //    table: "StepTaskAnswers");

            migrationBuilder.AddColumn<bool>(
                name: "IsStakeholder",
                table: "TextAnswers",
                nullable: false,
                defaultValue: false);

            //migrationBuilder.AlterColumn<string>(
            //    name: "LastName",
            //    table: "StepTaskAnswers",
            //    maxLength: 50,
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldMaxLength: 50,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "FirstName",
            //    table: "StepTaskAnswers",
            //    maxLength: 50,
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldMaxLength: 50,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Email",
            //    table: "StepTaskAnswers",
            //    maxLength: 50,
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldMaxLength: 50,
            //    oldNullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "PlanId",
            //    table: "StepTaskAnswers",
            //    nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_StepTaskAnswers_PlanId",
            //    table: "StepTaskAnswers",
            //    column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_StakeholderRatingAnswers_TextAnswers_StakeholderId",
                table: "StakeholderRatingAnswers",
                column: "StakeholderId",
                principalTable: "TextAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_StepTaskAnswers_Plans_PlanId",
            //    table: "StepTaskAnswers",
            //    column: "PlanId",
            //    principalTable: "Plans",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }
    }
}
