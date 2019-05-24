using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class StakeholderAnswer_isInternalcategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsInternal",
                table: "StakeholderAnswers",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "StakeholderAnswers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StakeholderAnswers_CategoryId",
                table: "StakeholderAnswers",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_StakeholderAnswers_Dictionaries_CategoryId",
                table: "StakeholderAnswers",
                column: "CategoryId",
                principalTable: "Dictionaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StakeholderAnswers_Dictionaries_CategoryId",
                table: "StakeholderAnswers");

            migrationBuilder.DropIndex(
                name: "IX_StakeholderAnswers_CategoryId",
                table: "StakeholderAnswers");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "StakeholderAnswers");

            migrationBuilder.AlterColumn<string>(
                name: "IsInternal",
                table: "StakeholderAnswers",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);
        }
    }
}
