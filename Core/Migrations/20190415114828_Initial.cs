﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dictionaries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    HasPosition = table.Column<bool>(nullable: false),
                    HasCriterion = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    IsWithActionPlan = table.Column<bool>(nullable: true),
                    IsCompleted = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StepBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Step = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Instruction = table.Column<string>(maxLength: 500, nullable: true),
                    Order = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepBlocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PositionId = table.Column<int>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    Password = table.Column<string>(maxLength: 100, nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    Role = table.Column<string>(maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Dictionaries_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Dictionaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StepBlockId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Type = table.Column<string>(maxLength: 50, nullable: false),
                    HasOptions = table.Column<bool>(nullable: false, defaultValue: false),
                    HasFiles = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_StepBlocks_StepBlockId",
                        column: x => x.StepBlockId,
                        principalTable: "StepBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersToPlans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    PlanId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersToPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersToPlans_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersToPlans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Path = table.Column<string>(maxLength: 500, nullable: false),
                    Ext = table.Column<string>(maxLength: 50, nullable: false),
                    QuestionId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Options",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuestionId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Options", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Options_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BooleanAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserToPlanId = table.Column<int>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false),
                    IsFinal = table.Column<bool>(nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    ResourceId = table.Column<int>(nullable: true),
                    Answer = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BooleanAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BooleanAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BooleanAnswers_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BooleanAnswers_UsersToPlans_UserToPlanId",
                        column: x => x.UserToPlanId,
                        principalTable: "UsersToPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StepAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserToPlanId = table.Column<int>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false),
                    IsFinal = table.Column<bool>(nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    Step = table.Column<string>(maxLength: 50, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Remind = table.Column<int>(nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StepAnswers_UsersToPlans_UserToPlanId",
                        column: x => x.UserToPlanId,
                        principalTable: "UsersToPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TextAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserToPlanId = table.Column<int>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false),
                    IsFinal = table.Column<bool>(nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    Text = table.Column<string>(maxLength: 500, nullable: false),
                    IsIssue = table.Column<bool>(nullable: false, defaultValue: false),
                    IsStakeholder = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TextAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TextAnswers_UsersToPlans_UserToPlanId",
                        column: x => x.UserToPlanId,
                        principalTable: "UsersToPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Introductions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PlanId = table.Column<int>(nullable: false),
                    VideoId = table.Column<int>(nullable: false),
                    Step = table.Column<string>(maxLength: 50, nullable: false),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Introductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Introductions_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Introductions_Files_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssueOptionAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserToPlanId = table.Column<int>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false),
                    IsFinal = table.Column<bool>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    IssueId = table.Column<int>(nullable: false),
                    Option = table.Column<string>(maxLength: 50, nullable: false),
                    IsBestOption = table.Column<bool>(nullable: false, defaultValue: false),
                    Actors = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueOptionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueOptionAnswers_TextAnswers_IssueId",
                        column: x => x.IssueId,
                        principalTable: "TextAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssueOptionAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssueOptionAnswers_UsersToPlans_UserToPlanId",
                        column: x => x.UserToPlanId,
                        principalTable: "UsersToPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SelectAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserToPlanId = table.Column<int>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false),
                    IsFinal = table.Column<bool>(nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    OptionId = table.Column<int>(nullable: true),
                    IssueId = table.Column<int>(nullable: true),
                    AltOption = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectAnswers_TextAnswers_IssueId",
                        column: x => x.IssueId,
                        principalTable: "TextAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SelectAnswers_Options_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Options",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SelectAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SelectAnswers_UsersToPlans_UserToPlanId",
                        column: x => x.UserToPlanId,
                        principalTable: "UsersToPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StakeholderRatingAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserToPlanId = table.Column<int>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false),
                    IsFinal = table.Column<bool>(nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    StakeholderId = table.Column<int>(nullable: false),
                    Grade = table.Column<int>(nullable: true),
                    Priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StakeholderRatingAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StakeholderRatingAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StakeholderRatingAnswers_TextAnswers_StakeholderId",
                        column: x => x.StakeholderId,
                        principalTable: "TextAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StakeholderRatingAnswers_UsersToPlans_UserToPlanId",
                        column: x => x.UserToPlanId,
                        principalTable: "UsersToPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StrategicIssueAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserToPlanId = table.Column<int>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false),
                    IsFinal = table.Column<bool>(nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    IssueId = table.Column<int>(nullable: false),
                    Why = table.Column<string>(maxLength: 500, nullable: false),
                    Result = table.Column<string>(maxLength: 500, nullable: false),
                    Goal = table.Column<string>(maxLength: 500, nullable: false),
                    Solution = table.Column<string>(maxLength: 500, nullable: false),
                    Ranking = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategicIssueAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StrategicIssueAnswers_TextAnswers_IssueId",
                        column: x => x.IssueId,
                        principalTable: "TextAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StrategicIssueAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StrategicIssueAnswers_UsersToPlans_UserToPlanId",
                        column: x => x.UserToPlanId,
                        principalTable: "UsersToPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssueOptionAnswersToResources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IssueOptionAnswerId = table.Column<int>(nullable: false),
                    ResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueOptionAnswersToResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueOptionAnswersToResources_IssueOptionAnswers_IssueOptionAnswerId",
                        column: x => x.IssueOptionAnswerId,
                        principalTable: "IssueOptionAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueOptionAnswersToResources_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreparingAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserToPlanId = table.Column<int>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false),
                    IsFinal = table.Column<bool>(nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    IssueOptionAnswerId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    HowItWillBeDone = table.Column<string>(maxLength: 500, nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreparingAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreparingAnswers_IssueOptionAnswers_IssueOptionAnswerId",
                        column: x => x.IssueOptionAnswerId,
                        principalTable: "IssueOptionAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreparingAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreparingAnswers_UsersToPlans_UserToPlanId",
                        column: x => x.UserToPlanId,
                        principalTable: "UsersToPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsersToIssueOptionAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    IssueOptionAnswerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersToIssueOptionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersToIssueOptionAnswers_IssueOptionAnswers_IssueOptionAnswerId",
                        column: x => x.IssueOptionAnswerId,
                        principalTable: "IssueOptionAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersToIssueOptionAnswers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StakeholderRatingAnswersToDictionaries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StakeholderRatingAnswerId = table.Column<int>(nullable: false),
                    CriterionId = table.Column<int>(nullable: false),
                    Rate = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StakeholderRatingAnswersToDictionaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StakeholderRatingAnswersToDictionaries_Dictionaries_CriterionId",
                        column: x => x.CriterionId,
                        principalTable: "Dictionaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StakeholderRatingAnswersToDictionaries_StakeholderRatingAnswers_StakeholderRatingAnswerId",
                        column: x => x.StakeholderRatingAnswerId,
                        principalTable: "StakeholderRatingAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BooleanAnswers_QuestionId",
                table: "BooleanAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanAnswers_ResourceId",
                table: "BooleanAnswers",
                column: "ResourceId",
                unique: true,
                filter: "[ResourceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanAnswers_UserToPlanId",
                table: "BooleanAnswers",
                column: "UserToPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_QuestionId",
                table: "Files",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Introductions_PlanId",
                table: "Introductions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Introductions_VideoId",
                table: "Introductions",
                column: "VideoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssueOptionAnswers_IssueId",
                table: "IssueOptionAnswers",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueOptionAnswers_QuestionId",
                table: "IssueOptionAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueOptionAnswers_UserToPlanId",
                table: "IssueOptionAnswers",
                column: "UserToPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueOptionAnswersToResources_IssueOptionAnswerId",
                table: "IssueOptionAnswersToResources",
                column: "IssueOptionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueOptionAnswersToResources_ResourceId",
                table: "IssueOptionAnswersToResources",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Options_QuestionId",
                table: "Options",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_PreparingAnswers_IssueOptionAnswerId",
                table: "PreparingAnswers",
                column: "IssueOptionAnswerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreparingAnswers_QuestionId",
                table: "PreparingAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_PreparingAnswers_UserToPlanId",
                table: "PreparingAnswers",
                column: "UserToPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_StepBlockId",
                table: "Questions",
                column: "StepBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectAnswers_IssueId",
                table: "SelectAnswers",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectAnswers_OptionId",
                table: "SelectAnswers",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectAnswers_QuestionId",
                table: "SelectAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectAnswers_UserToPlanId",
                table: "SelectAnswers",
                column: "UserToPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_StakeholderRatingAnswers_QuestionId",
                table: "StakeholderRatingAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_StakeholderRatingAnswers_StakeholderId",
                table: "StakeholderRatingAnswers",
                column: "StakeholderId");

            migrationBuilder.CreateIndex(
                name: "IX_StakeholderRatingAnswers_UserToPlanId",
                table: "StakeholderRatingAnswers",
                column: "UserToPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_StakeholderRatingAnswersToDictionaries_CriterionId",
                table: "StakeholderRatingAnswersToDictionaries",
                column: "CriterionId");

            migrationBuilder.CreateIndex(
                name: "IX_StakeholderRatingAnswersToDictionaries_StakeholderRatingAnswerId",
                table: "StakeholderRatingAnswersToDictionaries",
                column: "StakeholderRatingAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_StepAnswers_QuestionId",
                table: "StepAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_StepAnswers_UserToPlanId",
                table: "StepAnswers",
                column: "UserToPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_StrategicIssueAnswers_IssueId",
                table: "StrategicIssueAnswers",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_StrategicIssueAnswers_QuestionId",
                table: "StrategicIssueAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_StrategicIssueAnswers_UserToPlanId",
                table: "StrategicIssueAnswers",
                column: "UserToPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TextAnswers_QuestionId",
                table: "TextAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TextAnswers_UserToPlanId",
                table: "TextAnswers",
                column: "UserToPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PositionId",
                table: "Users",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersToIssueOptionAnswers_IssueOptionAnswerId",
                table: "UsersToIssueOptionAnswers",
                column: "IssueOptionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersToIssueOptionAnswers_UserId",
                table: "UsersToIssueOptionAnswers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersToPlans_PlanId",
                table: "UsersToPlans",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersToPlans_UserId",
                table: "UsersToPlans",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BooleanAnswers");

            migrationBuilder.DropTable(
                name: "Introductions");

            migrationBuilder.DropTable(
                name: "IssueOptionAnswersToResources");

            migrationBuilder.DropTable(
                name: "PreparingAnswers");

            migrationBuilder.DropTable(
                name: "SelectAnswers");

            migrationBuilder.DropTable(
                name: "StakeholderRatingAnswersToDictionaries");

            migrationBuilder.DropTable(
                name: "StepAnswers");

            migrationBuilder.DropTable(
                name: "StrategicIssueAnswers");

            migrationBuilder.DropTable(
                name: "UsersToIssueOptionAnswers");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "Options");

            migrationBuilder.DropTable(
                name: "StakeholderRatingAnswers");

            migrationBuilder.DropTable(
                name: "IssueOptionAnswers");

            migrationBuilder.DropTable(
                name: "TextAnswers");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "UsersToPlans");

            migrationBuilder.DropTable(
                name: "StepBlocks");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Dictionaries");
        }
    }
}
