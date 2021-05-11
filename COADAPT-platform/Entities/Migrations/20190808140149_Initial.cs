using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Entities.Migrations {
	public partial class Initial : Migration {
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.CreateTable(
				name: "AspNetRoles",
				columns: table => new {
					Id = table.Column<string>(nullable: false),
					Name = table.Column<string>(maxLength: 256, nullable: true),
					NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
					ConcurrencyStamp = table.Column<string>(nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_AspNetRoles", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUsers",
				columns: table => new {
					Id = table.Column<string>(nullable: false),
					UserName = table.Column<string>(maxLength: 256, nullable: true),
					NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
					Email = table.Column<string>(maxLength: 256, nullable: true),
					NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
					EmailConfirmed = table.Column<bool>(nullable: false),
					PasswordHash = table.Column<string>(nullable: true),
					SecurityStamp = table.Column<string>(nullable: true),
					ConcurrencyStamp = table.Column<string>(nullable: true),
					PhoneNumber = table.Column<string>(nullable: true),
					PhoneNumberConfirmed = table.Column<bool>(nullable: false),
					TwoFactorEnabled = table.Column<bool>(nullable: false),
					LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
					LockoutEnabled = table.Column<bool>(nullable: false),
					AccessFailedCount = table.Column<int>(nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_AspNetUsers", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "AspNetRoleClaims",
				columns: table => new {
					Id = table.Column<int>(nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					RoleId = table.Column<string>(nullable: false),
					ClaimType = table.Column<string>(nullable: true),
					ClaimValue = table.Column<string>(nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Administrators",
				columns: table => new {
					Id = table.Column<int>(nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					CreatedOn = table.Column<DateTime>(type: "DATETIME", nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					UserId = table.Column<string>(nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_Administrators", x => x.Id);
					table.ForeignKey(
						name: "FK_Administrators_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserClaims",
				columns: table => new {
					Id = table.Column<int>(nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					UserId = table.Column<string>(nullable: false),
					ClaimType = table.Column<string>(nullable: true),
					ClaimValue = table.Column<string>(nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetUserClaims_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserLogins",
				columns: table => new {
					LoginProvider = table.Column<string>(nullable: false),
					ProviderKey = table.Column<string>(nullable: false),
					ProviderDisplayName = table.Column<string>(nullable: true),
					UserId = table.Column<string>(nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
					table.ForeignKey(
						name: "FK_AspNetUserLogins_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserRoles",
				columns: table => new {
					UserId = table.Column<string>(nullable: false),
					RoleId = table.Column<string>(nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserTokens",
				columns: table => new {
					UserId = table.Column<string>(nullable: false),
					LoginProvider = table.Column<string>(nullable: false),
					Name = table.Column<string>(nullable: false),
					Value = table.Column<string>(nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
					table.ForeignKey(
						name: "FK_AspNetUserTokens_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "SubAdministrators",
				columns: table => new {
					Id = table.Column<int>(nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					CreatedOn = table.Column<DateTime>(type: "DATETIME", nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					UserId = table.Column<string>(nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_SubAdministrators", x => x.Id);
					table.ForeignKey(
						name: "FK_SubAdministrators_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "Supervisors",
				columns: table => new {
					Id = table.Column<int>(nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					CreatedOn = table.Column<DateTime>(type: "DATETIME", nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					UserId = table.Column<string>(nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_Supervisors", x => x.Id);
					table.ForeignKey(
						name: "FK_Supervisors_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "Therapists",
				columns: table => new {
					Id = table.Column<int>(nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					CreatedOn = table.Column<DateTime>(type: "DATETIME", nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					UserId = table.Column<string>(nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_Therapists", x => x.Id);
					table.ForeignKey(
						name: "FK_Therapists_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "Organizations",
				columns: table => new {
					Id = table.Column<int>(nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					Name = table.Column<string>(nullable: false),
					Shortname = table.Column<string>(nullable: false),
					SubAdministratorId = table.Column<int>(nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_Organizations", x => x.Id);
					table.ForeignKey(
						name: "FK_Organizations_SubAdministrators_SubAdministratorId",
						column: x => x.SubAdministratorId,
						principalTable: "SubAdministrators",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Participants",
				columns: table => new {
					Id = table.Column<int>(nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					CreatedOn = table.Column<DateTime>(type: "DATETIME", nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					Code = table.Column<string>(nullable: false),
					UserId = table.Column<string>(nullable: true),
					TherapistId = table.Column<int>(nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_Participants", x => x.Id);
					table.ForeignKey(
						name: "FK_Participants_Therapists_TherapistId",
						column: x => x.TherapistId,
						principalTable: "Therapists",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Participants_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "Studies",
				columns: table => new {
					Id = table.Column<int>(nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					Name = table.Column<string>(nullable: false),
					Shortname = table.Column<string>(nullable: false),
					OrganizationId = table.Column<int>(nullable: false),
					SupervisorId = table.Column<int>(nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_Studies", x => x.Id);
					table.ForeignKey(
						name: "FK_Studies_Organizations_OrganizationId",
						column: x => x.OrganizationId,
						principalTable: "Organizations",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Studies_Supervisors_SupervisorId",
						column: x => x.SupervisorId,
						principalTable: "Supervisors",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Sites",
				columns: table => new {
					Id = table.Column<int>(nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					Name = table.Column<string>(nullable: false),
					Shortname = table.Column<string>(nullable: false),
					StudyId = table.Column<int>(nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_Sites", x => x.Id);
					table.ForeignKey(
						name: "FK_Sites_Studies_StudyId",
						column: x => x.StudyId,
						principalTable: "Studies",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "SiteParticipants",
				columns: table => new {
					SiteId = table.Column<int>(nullable: false),
					ParticipantId = table.Column<int>(nullable: false)
				},
				constraints: table => {
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
				name: "IX_Administrators_UserId",
				table: "Administrators",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetRoleClaims_RoleId",
				table: "AspNetRoleClaims",
				column: "RoleId");

			migrationBuilder.CreateIndex(
				name: "RoleNameIndex",
				table: "AspNetRoles",
				column: "NormalizedName",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserClaims_UserId",
				table: "AspNetUserClaims",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserLogins_UserId",
				table: "AspNetUserLogins",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserRoles_RoleId",
				table: "AspNetUserRoles",
				column: "RoleId");

			migrationBuilder.CreateIndex(
				name: "EmailIndex",
				table: "AspNetUsers",
				column: "NormalizedEmail");

			migrationBuilder.CreateIndex(
				name: "UserNameIndex",
				table: "AspNetUsers",
				column: "NormalizedUserName",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Organizations_SubAdministratorId",
				table: "Organizations",
				column: "SubAdministratorId");

			migrationBuilder.CreateIndex(
				name: "IX_Participants_TherapistId",
				table: "Participants",
				column: "TherapistId");

			migrationBuilder.CreateIndex(
				name: "IX_Participants_UserId",
				table: "Participants",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_SiteParticipants_ParticipantId",
				table: "SiteParticipants",
				column: "ParticipantId");

			migrationBuilder.CreateIndex(
				name: "IX_Sites_StudyId",
				table: "Sites",
				column: "StudyId");

			migrationBuilder.CreateIndex(
				name: "IX_Studies_OrganizationId",
				table: "Studies",
				column: "OrganizationId");

			migrationBuilder.CreateIndex(
				name: "IX_Studies_SupervisorId",
				table: "Studies",
				column: "SupervisorId");

			migrationBuilder.CreateIndex(
				name: "IX_SubAdministrators_UserId",
				table: "SubAdministrators",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_Supervisors_UserId",
				table: "Supervisors",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_Therapists_UserId",
				table: "Therapists",
				column: "UserId");
		}

		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropTable(
				name: "Administrators");

			migrationBuilder.DropTable(
				name: "AspNetRoleClaims");

			migrationBuilder.DropTable(
				name: "AspNetUserClaims");

			migrationBuilder.DropTable(
				name: "AspNetUserLogins");

			migrationBuilder.DropTable(
				name: "AspNetUserRoles");

			migrationBuilder.DropTable(
				name: "AspNetUserTokens");

			migrationBuilder.DropTable(
				name: "SiteParticipants");

			migrationBuilder.DropTable(
				name: "AspNetRoles");

			migrationBuilder.DropTable(
				name: "Participants");

			migrationBuilder.DropTable(
				name: "Sites");

			migrationBuilder.DropTable(
				name: "Therapists");

			migrationBuilder.DropTable(
				name: "Studies");

			migrationBuilder.DropTable(
				name: "Organizations");

			migrationBuilder.DropTable(
				name: "Supervisors");

			migrationBuilder.DropTable(
				name: "SubAdministrators");

			migrationBuilder.DropTable(
				name: "AspNetUsers");
		}
	}
}
