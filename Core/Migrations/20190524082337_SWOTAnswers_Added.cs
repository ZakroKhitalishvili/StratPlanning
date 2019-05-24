using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class SWOTAnswers_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueOptionAnswers_TextAnswers_IssueId",
                table: "IssueOptionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_SelectAnswers_TextAnswers_IssueId",
                table: "SelectAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StrategicIssueAnswers_TextAnswers_IssueId",
                table: "StrategicIssueAnswers");

            migrationBuilder.DropColumn(
                name: "IsIssue",
                table: "TextAnswers");

            //migrationBuilder.AlterColumn<bool>(
            //    name: "HasPosition",
            //    table: "Dictionaries",
            //    nullable: false,
            //    defaultValue: false,
            //    oldClrType: typeof(bool));

            //migrationBuilder.AlterColumn<bool>(
            //    name: "HasCriterion",
            //    table: "Dictionaries",
            //    nullable: false,
            //    defaultValue: false,
            //    oldClrType: typeof(bool));

            //migrationBuilder.AddColumn<bool>(
            //    name: "HasValue",
            //    table: "Dictionaries",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SWOTAnswer",
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
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    Type = table.Column<string>(nullable: true),
                    IsIssue = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SWOTAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SWOTAnswer_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SWOTAnswer_UserStepResults_UserStepResultId",
                        column: x => x.UserStepResultId,
                        principalTable: "UserStepResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SWOTAnswer_QuestionId",
                table: "SWOTAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SWOTAnswer_UserStepResultId",
                table: "SWOTAnswer",
                column: "UserStepResultId");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropTable(
                name: "SWOTAnswer");

            migrationBuilder.DropColumn(
                name: "HasValue",
                table: "Dictionaries");

            migrationBuilder.AddColumn<bool>(
                name: "IsIssue",
                table: "TextAnswers",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.AddForeignKey(
                name: "FK_IssueOptionAnswers_TextAnswers_IssueId",
                table: "IssueOptionAnswers",
                column: "IssueId",
                principalTable: "TextAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SelectAnswers_TextAnswers_IssueId",
                table: "SelectAnswers",
                column: "IssueId",
                principalTable: "TextAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StrategicIssueAnswers_TextAnswers_IssueId",
                table: "StrategicIssueAnswers",
                column: "IssueId",
                principalTable: "TextAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
