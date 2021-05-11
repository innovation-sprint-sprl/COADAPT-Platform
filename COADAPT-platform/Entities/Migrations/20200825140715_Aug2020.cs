using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Entities.Migrations
{
    public partial class Aug2020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteParticipants");

            migrationBuilder.AddColumn<DateTime>(
                name: "Birthday",
                table: "Therapists",
                type: "DATETIME",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Therapists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Therapists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BirthPlace",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
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

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
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
                name: "OuraActivities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParticipantId = table.Column<int>(nullable: false),
                    SummaryDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    TimezoneOffset = table.Column<int>(nullable: false),
                    DayStart = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    DayEnd = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    Score = table.Column<int>(nullable: false),
                    ScoreStayActive = table.Column<int>(nullable: false),
                    ScoreMoveEveryHour = table.Column<int>(nullable: false),
                    ScoreMeetDailyTargets = table.Column<int>(nullable: false),
                    ScoreTrainingFrequency = table.Column<int>(nullable: false),
                    ScoreTrainingVolume = table.Column<int>(nullable: false),
                    ScoreRecoveryTime = table.Column<int>(nullable: false),
                    DailyMovement = table.Column<int>(nullable: false),
                    NonWear = table.Column<int>(nullable: false),
                    Rest = table.Column<int>(nullable: false),
                    Inactive = table.Column<int>(nullable: false),
                    InactivityAlerts = table.Column<int>(nullable: false),
                    Low = table.Column<int>(nullable: false),
                    Medium = table.Column<int>(nullable: false),
                    High = table.Column<int>(nullable: false),
                    Steps = table.Column<int>(nullable: false),
                    CaloriesTotal = table.Column<int>(nullable: false),
                    CaloriesActive = table.Column<int>(nullable: false),
                    MetMinInactive = table.Column<int>(nullable: false),
                    MetMinLow = table.Column<int>(nullable: false),
                    MetMinMedium = table.Column<int>(nullable: false),
                    MetMinMediumPlus = table.Column<int>(nullable: false),
                    MetMinHigh = table.Column<int>(nullable: false),
                    AverageMet = table.Column<int>(nullable: false),
                    Class5Min = table.Column<string>(nullable: true),
                    Met1Min = table.Column<string>(nullable: true),
                    TargetCalories = table.Column<int>(nullable: false),
                    TargetKm = table.Column<int>(nullable: false),
                    TargetMiles = table.Column<int>(nullable: false),
                    ToTargetKm = table.Column<int>(nullable: false),
                    ToTargetMiles = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OuraActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OuraActivities_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OuraReadinesses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParticipantId = table.Column<int>(nullable: false),
                    SummaryDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    PeriodId = table.Column<int>(nullable: false),
                    Score = table.Column<int>(nullable: false),
                    ScorePreviousNight = table.Column<int>(nullable: false),
                    ScoreSleepBalance = table.Column<int>(nullable: false),
                    ScorePreviousDay = table.Column<int>(nullable: false),
                    ScoreActivityBalance = table.Column<int>(nullable: false),
                    ScoreRestingHr = table.Column<int>(nullable: false),
                    ScoreHrvBalance = table.Column<int>(nullable: false),
                    ScoreRecoveryIndex = table.Column<int>(nullable: false),
                    ScoreTemperature = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OuraReadinesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OuraReadinesses_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OuraSleeps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParticipantId = table.Column<int>(nullable: false),
                    SummaryDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    PeriodId = table.Column<int>(nullable: false),
                    TimezoneOffset = table.Column<int>(nullable: false),
                    BedtimeStart = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    BedtimeEnd = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    Score = table.Column<int>(nullable: false),
                    ScoreTotal = table.Column<int>(nullable: false),
                    ScoreDisturbances = table.Column<int>(nullable: false),
                    ScoreEfficiency = table.Column<int>(nullable: false),
                    ScoreLatency = table.Column<int>(nullable: false),
                    ScoreRem = table.Column<int>(nullable: false),
                    ScoreDeep = table.Column<int>(nullable: false),
                    ScoreAlignment = table.Column<int>(nullable: false),
                    Total = table.Column<int>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    Awake = table.Column<int>(nullable: false),
                    Light = table.Column<int>(nullable: false),
                    Rem = table.Column<int>(nullable: false),
                    Deep = table.Column<int>(nullable: false),
                    OnsetLatency = table.Column<int>(nullable: false),
                    Restless = table.Column<int>(nullable: false),
                    Efficiency = table.Column<int>(nullable: false),
                    MidpointTime = table.Column<int>(nullable: false),
                    HrLowest = table.Column<int>(nullable: false),
                    HrAverage = table.Column<int>(nullable: false),
                    RmsSd = table.Column<int>(nullable: false),
                    BreathAverage = table.Column<int>(nullable: false),
                    TemperatureDelta = table.Column<int>(nullable: false),
                    Hypnogram5Min = table.Column<string>(nullable: true),
                    Hr5Min = table.Column<string>(nullable: true),
                    RmsSd5Min = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OuraSleeps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OuraSleeps_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantInfos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParticipantId = table.Column<int>(nullable: false),
                    DateOfReport = table.Column<DateTime>(type: "DATETIME", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MaritalStatus = table.Column<string>(nullable: true),
                    NumberOfChildren = table.Column<int>(nullable: false),
                    ThoughtsOnChildren = table.Column<string>(nullable: true),
                    ThoughtsOnParents = table.Column<string>(nullable: true),
                    DateOfCurrentJob = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    JobType = table.Column<string>(nullable: true),
                    StressfulEvents = table.Column<string>(nullable: true),
                    MedicalConditionSelfAssessment = table.Column<string>(nullable: true),
                    Substances = table.Column<string>(nullable: true),
                    PsychologicalConditionSelfAssessment = table.Column<string>(nullable: true),
                    RegionOfResidence = table.Column<string>(nullable: true),
                    PlaceOfResidence = table.Column<string>(nullable: true),
                    Files = table.Column<string>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "PhysiologicalSignals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParticipantId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Value = table.Column<float>(nullable: false),
                    Accuracy = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysiologicalSignals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhysiologicalSignals_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PsychologicalReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParticipantId = table.Column<int>(nullable: false),
                    DateOfReport = table.Column<DateTime>(type: "DATETIME", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IntrinsicFactorsOfWork = table.Column<int>(nullable: false),
                    ManagerialRole = table.Column<int>(nullable: false),
                    RelationshipWithOtherPeople = table.Column<int>(nullable: false),
                    CareerAndSuccess = table.Column<int>(nullable: false),
                    ClimateAndOrganizationalStructure = table.Column<int>(nullable: false),
                    HomeWorkInterface = table.Column<int>(nullable: false),
                    AttitudeTowardsLife = table.Column<int>(nullable: false),
                    StyleOfBehavior = table.Column<int>(nullable: false),
                    Ambition = table.Column<int>(nullable: false),
                    TypeASynthetic = table.Column<int>(nullable: false),
                    TypeATotal = table.Column<int>(nullable: false),
                    OrganizationalForces = table.Column<int>(nullable: false),
                    ManagementProcesses = table.Column<int>(nullable: false),
                    IndividualInfluences = table.Column<int>(nullable: false),
                    LocusOfControlSynthetic = table.Column<int>(nullable: false),
                    LocusOfControlTotal = table.Column<int>(nullable: false),
                    SocialSupport = table.Column<int>(nullable: false),
                    TaskOrientation = table.Column<int>(nullable: false),
                    Logical = table.Column<int>(nullable: false),
                    HomeWorkRelationship = table.Column<int>(nullable: false),
                    Time = table.Column<int>(nullable: false),
                    Involvement = table.Column<int>(nullable: false),
                    CareerSatisfaction = table.Column<int>(nullable: false),
                    JobSatisfaction = table.Column<int>(nullable: false),
                    SettingAndOrganizationalStructureSatisfaction = table.Column<int>(nullable: false),
                    OrganizationalProcessesSatisfaction = table.Column<int>(nullable: false),
                    InterpersonalRelationshipsSatisfaction = table.Column<int>(nullable: false),
                    SyntheticJobSatisfaction = table.Column<int>(nullable: false),
                    TotalJobSatisfaction = table.Column<int>(nullable: false),
                    PsychologicalHealth = table.Column<int>(nullable: false),
                    PhysicalHealth = table.Column<int>(nullable: false),
                    GlobalSeverityIndex = table.Column<int>(nullable: false),
                    PositiveSymptomTotal = table.Column<int>(nullable: false),
                    PositiveSymptomDistressIndex = table.Column<int>(nullable: false),
                    Somatization = table.Column<int>(nullable: false),
                    ObsessivenessCompulsiveness = table.Column<int>(nullable: false),
                    InterpersonalHypersensitivity = table.Column<int>(nullable: false),
                    Depression = table.Column<int>(nullable: false),
                    Anxiety = table.Column<int>(nullable: false),
                    Hostility = table.Column<int>(nullable: false),
                    PhobicAnxiety = table.Column<int>(nullable: false),
                    ParanoidIdeation = table.Column<int>(nullable: false),
                    Psychoticism = table.Column<int>(nullable: false),
                    PerceivedStressScale = table.Column<int>(nullable: false),
                    SleepProblem = table.Column<int>(nullable: false),
                    TrialUserComments = table.Column<string>(nullable: true),
                    TherapistComments = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsychologicalReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PsychologicalReports_Participants_ParticipantId",
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
                    EndDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    Abandoned = table.Column<bool>(nullable: false),
                    DataCollectionTurn = table.Column<int>(nullable: false),
                    PlaceOfConsent = table.Column<string>(nullable: true)
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
                name: "IX_OuraActivities_ParticipantId",
                table: "OuraActivities",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_OuraReadinesses_ParticipantId",
                table: "OuraReadinesses",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_OuraSleeps_ParticipantId",
                table: "OuraSleeps",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantInfos_ParticipantId",
                table: "ParticipantInfos",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysiologicalSignals_ParticipantId",
                table: "PhysiologicalSignals",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_PsychologicalReports_ParticipantId",
                table: "PsychologicalReports",
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
                name: "OuraActivities");

            migrationBuilder.DropTable(
                name: "OuraReadinesses");

            migrationBuilder.DropTable(
                name: "OuraSleeps");

            migrationBuilder.DropTable(
                name: "ParticipantInfos");

            migrationBuilder.DropTable(
                name: "PhysiologicalSignals");

            migrationBuilder.DropTable(
                name: "PsychologicalReports");

            migrationBuilder.DropTable(
                name: "StudyParticipants");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropColumn(
                name: "Birthday",
                table: "Therapists");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Therapists");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Therapists");

            migrationBuilder.DropColumn(
                name: "BirthPlace",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "DateOfFirstJob",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "Language",
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
