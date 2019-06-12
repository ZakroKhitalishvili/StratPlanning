using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class StakeholderRatingAnswersToDictionaries_OnDelete_Restricted_To_Dictionaries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StakeholderRatingAnswersToDictionaries_Dictionaries_CriterionId",
                table: "StakeholderRatingAnswersToDictionaries");

            migrationBuilder.AddForeignKey(
                name: "FK_StakeholderRatingAnswersToDictionaries_Dictionaries_CriterionId",
                table: "StakeholderRatingAnswersToDictionaries",
                column: "CriterionId",
                principalTable: "Dictionaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StakeholderRatingAnswersToDictionaries_Dictionaries_CriterionId",
                table: "StakeholderRatingAnswersToDictionaries");

            migrationBuilder.AddForeignKey(
                name: "FK_StakeholderRatingAnswersToDictionaries_Dictionaries_CriterionId",
                table: "StakeholderRatingAnswersToDictionaries",
                column: "CriterionId",
                principalTable: "Dictionaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
