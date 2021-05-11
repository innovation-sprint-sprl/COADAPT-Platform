using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Entities.Migrations
{
    public partial class Sept2020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParticipantInfos");

            migrationBuilder.RenameColumn(
                name: "Education",
                table: "Participants",
                newName: "Files");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfCurrentJob",
                table: "StudyParticipants",
                type: "DATETIME",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "StudyParticipants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobType",
                table: "StudyParticipants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "StudyParticipants",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfChildren",
                table: "StudyParticipants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PlaceOfResidence",
                table: "StudyParticipants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegionOfResidence",
                table: "StudyParticipants",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfReport",
                table: "PsychologicalReports",
                type: "DATETIME",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "MedicalConditionSelfAssessment",
                table: "PsychologicalReports",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PsychologicalConditionSelfAssessment",
                table: "PsychologicalReports",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StressfulEvents",
                table: "PsychologicalReports",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThoughtsOnChildren",
                table: "PsychologicalReports",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThoughtsOnParents",
                table: "PsychologicalReports",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "PhysiologicalSignals",
                type: "DATETIME(3)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfCurrentJob",
                table: "StudyParticipants");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "StudyParticipants");

            migrationBuilder.DropColumn(
                name: "JobType",
                table: "StudyParticipants");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "StudyParticipants");

            migrationBuilder.DropColumn(
                name: "NumberOfChildren",
                table: "StudyParticipants");

            migrationBuilder.DropColumn(
                name: "PlaceOfResidence",
                table: "StudyParticipants");

            migrationBuilder.DropColumn(
                name: "RegionOfResidence",
                table: "StudyParticipants");

            migrationBuilder.DropColumn(
                name: "MedicalConditionSelfAssessment",
                table: "PsychologicalReports");

            migrationBuilder.DropColumn(
                name: "PsychologicalConditionSelfAssessment",
                table: "PsychologicalReports");

            migrationBuilder.DropColumn(
                name: "StressfulEvents",
                table: "PsychologicalReports");

            migrationBuilder.DropColumn(
                name: "ThoughtsOnChildren",
                table: "PsychologicalReports");

            migrationBuilder.DropColumn(
                name: "ThoughtsOnParents",
                table: "PsychologicalReports");

            migrationBuilder.RenameColumn(
                name: "Files",
                table: "Participants",
                newName: "Education");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfReport",
                table: "PsychologicalReports",
                type: "DATETIME",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "PhysiologicalSignals",
                type: "DATETIME",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME(3)");

            migrationBuilder.CreateTable(
                name: "ParticipantInfos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DateOfCurrentJob = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    DateOfReport = table.Column<DateTime>(type: "DATETIME", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Files = table.Column<string>(nullable: true),
                    JobType = table.Column<string>(nullable: true),
                    MaritalStatus = table.Column<string>(nullable: true),
                    MedicalConditionSelfAssessment = table.Column<string>(nullable: true),
                    NumberOfChildren = table.Column<int>(nullable: false),
                    ParticipantId = table.Column<int>(nullable: false),
                    PlaceOfResidence = table.Column<string>(nullable: true),
                    PsychologicalConditionSelfAssessment = table.Column<string>(nullable: true),
                    RegionOfResidence = table.Column<string>(nullable: true),
                    StressfulEvents = table.Column<string>(nullable: true),
                    Substances = table.Column<string>(nullable: true),
                    ThoughtsOnChildren = table.Column<string>(nullable: true),
                    ThoughtsOnParents = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipantInfos_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantInfos_ParticipantId",
                table: "ParticipantInfos",
                column: "ParticipantId");
        }
    }
}
