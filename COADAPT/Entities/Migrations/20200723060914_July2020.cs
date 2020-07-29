using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Entities.Migrations
{
    public partial class July2020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteParticipants");

            migrationBuilder.AddColumn<string>(
                name: "Children",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Participants",
                type: "DATETIME",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfCurrentJob",
                table: "Participants",
                type: "DATETIME",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfFirstJob",
                table: "Participants",
                type: "DATETIME",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Participants",
                type: "DATETIME",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "JobType",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedicalCondition",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Parents",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhsychologicalCondition",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sex",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Participants",
                type: "DATETIME",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "StressfulEvents",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Substances",
                table: "Participants",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppUsageLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReportedOn = table.Column<DateTime>(type: "DATETIME", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    Tag = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsageLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Shortname = table.Column<string>(nullable: false),
                    StudyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalInformations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParticipantId = table.Column<int>(nullable: false),
                    DateOfReport = table.Column<DateTime>(type: "DATETIME", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MaritalStatus = table.Column<string>(nullable: true),
                    StressLevelInMonth = table.Column<int>(nullable: true),
                    IntrinsicFactorsOfWork = table.Column<int>(nullable: true),
                    ManagerialRole = table.Column<int>(nullable: true),
                    RelationshipWithOtherPeople = table.Column<int>(nullable: true),
                    CareerAndSuccess = table.Column<int>(nullable: true),
                    ClimateAndOrganizationalStructure = table.Column<int>(nullable: true),
                    HomeWorkInterface = table.Column<int>(nullable: true),
                    AttitudeTowardsLife = table.Column<int>(nullable: true),
                    StyleOfBehavior = table.Column<int>(nullable: true),
                    Ambition = table.Column<int>(nullable: true),
                    TypeASynthetic = table.Column<int>(nullable: true),
                    TypeATotal = table.Column<int>(nullable: true),
                    OrganizationalForces = table.Column<int>(nullable: true),
                    ManagementProcesses = table.Column<int>(nullable: true),
                    IndividualInfluences = table.Column<int>(nullable: true),
                    LocusOfControlSynthetic = table.Column<int>(nullable: true),
                    LocusOfControlTotal = table.Column<int>(nullable: true),
                    SocialSupport = table.Column<int>(nullable: true),
                    TaskOrientation = table.Column<int>(nullable: true),
                    Logical = table.Column<int>(nullable: true),
                    HomeWorkRelationship = table.Column<int>(nullable: true),
                    Time = table.Column<int>(nullable: true),
                    Involvement = table.Column<int>(nullable: true),
                    CareerSatisfaction = table.Column<int>(nullable: true),
                    JobSatisfaction = table.Column<int>(nullable: true),
                    SettingAndOrganizationalStructureSatisfaction = table.Column<int>(nullable: true),
                    OrganizationalProcessesSatisfaction = table.Column<int>(nullable: true),
                    InterpersonalRelationshipsSatisfaction = table.Column<int>(nullable: true),
                    SyntheticJobSatisfaction = table.Column<int>(nullable: true),
                    TotalJobSatisfaction = table.Column<int>(nullable: true),
                    PsychologicalHealth = table.Column<int>(nullable: true),
                    PhysicalHealth = table.Column<int>(nullable: true),
                    GlobalSeverityIndex = table.Column<int>(nullable: true),
                    PositiveSymptomTotal = table.Column<int>(nullable: true),
                    PositiveSymptomDistressIndex = table.Column<int>(nullable: true),
                    Somatization = table.Column<int>(nullable: true),
                    ObsessivenessCompulsiveness = table.Column<int>(nullable: true),
                    InterpersonalHypersensitivity = table.Column<int>(nullable: true),
                    Depression = table.Column<int>(nullable: true),
                    Anxiety = table.Column<int>(nullable: true),
                    Hostility = table.Column<int>(nullable: true),
                    PhobicAnxiety = table.Column<int>(nullable: true),
                    ParanoidIdeation = table.Column<int>(nullable: true),
                    Psychoticism = table.Column<int>(nullable: true),
                    GeneralReportSCL = table.Column<int>(nullable: true),
                    ResearchGroup = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalInformations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalInformations_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyParticipants",
                columns: table => new
                {
                    StudyId = table.Column<int>(nullable: false),
                    ParticipantId = table.Column<int>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    GroupId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(type: "DATETIME", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EndDate = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyParticipants", x => new { x.StudyId, x.ParticipantId });
                    table.ForeignKey(
                        name: "FK_StudyParticipants_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyParticipants_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyParticipants_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyParticipants_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_StudyId",
                table: "Groups",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalInformations_ParticipantId",
                table: "PersonalInformations",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyParticipants_GroupId",
                table: "StudyParticipants",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyParticipants_ParticipantId",
                table: "StudyParticipants",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyParticipants_SiteId",
                table: "StudyParticipants",
                column: "SiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUsageLogs");

            migrationBuilder.DropTable(
                name: "PersonalInformations");

            migrationBuilder.DropTable(
                name: "StudyParticipants");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropColumn(
                name: "Children",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "DateOfCurrentJob",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "DateOfFirstJob",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "JobType",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "MedicalCondition",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "Parents",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "PhsychologicalCondition",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "Sex",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "StressfulEvents",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "Substances",
                table: "Participants");

            migrationBuilder.CreateTable(
                name: "SiteParticipants",
                columns: table => new
                {
                    SiteId = table.Column<int>(nullable: false),
                    ParticipantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteParticipants", x => new { x.SiteId, x.ParticipantId });
                    table.ForeignKey(
                        name: "FK_SiteParticipants_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SiteParticipants_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiteParticipants_ParticipantId",
                table: "SiteParticipants",
                column: "ParticipantId");
        }
    }
}
