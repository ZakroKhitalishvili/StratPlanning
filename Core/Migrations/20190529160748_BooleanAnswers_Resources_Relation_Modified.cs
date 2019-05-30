using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class BooleanAnswers_Resources_Relation_Modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BooleanAnswers_ResourceId",
                table: "BooleanAnswers");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanAnswers_ResourceId",
                table: "BooleanAnswers",
                column: "ResourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BooleanAnswers_ResourceId",
                table: "BooleanAnswers");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanAnswers_ResourceId",
                table: "BooleanAnswers",
                column: "ResourceId",
                unique: true,
                filter: "[ResourceId] IS NOT NULL");
        }
    }
}
