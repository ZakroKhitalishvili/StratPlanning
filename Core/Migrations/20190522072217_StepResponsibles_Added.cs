using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class StepResponsibles_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StepResponsibles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Step = table.Column<string>(maxLength: 50, nullable: false),
                    UserToPlanId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepResponsibles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepResponsibles_UsersToPlans_UserToPlanId",
                        column: x => x.UserToPlanId,
                        principalTable: "UsersToPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StepResponsibles_UserToPlanId",
                table: "StepResponsibles",
                column: "UserToPlanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StepResponsibles");
        }
    }
}
