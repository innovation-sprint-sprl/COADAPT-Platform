﻿// <auto-generated />
using System;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Entities.Migrations {
	[DbContext(typeof(COADAPTContext))]
	[Migration("20190808140149_Initial")]
	partial class Initial {
		protected override void BuildTargetModel(ModelBuilder modelBuilder) {
#pragma warning disable 612, 618
			modelBuilder
				.HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
				.HasAnnotation("Relational:MaxIdentifierLength", 64);

			modelBuilder.Entity("Entities.Models.Administrator", b => {
				b.Property<int>("Id")
					.ValueGeneratedOnAdd();

				b.Property<DateTime>("CreatedOn")
					.ValueGeneratedOnAdd()
					.HasColumnType("DATETIME");

				b.Property<string>("UserId");

				b.HasKey("Id");

				b.HasIndex("UserId");

				b.ToTable("Administrators");
			});

			modelBuilder.Entity("Entities.Models.Organization", b => {
				b.Property<int>("Id")
					.ValueGeneratedOnAdd();

				b.Property<string>("Name")
					.IsRequired();

				b.Property<string>("Shortname")
					.IsRequired();

				b.Property<int>("SubAdministratorId");

				b.HasKey("Id");

				b.HasIndex("SubAdministratorId");

				b.ToTable("Organizations");
			});

			modelBuilder.Entity("Entities.Models.Participant", b => {
				b.Property<int>("Id")
					.ValueGeneratedOnAdd();

				b.Property<string>("Code")
					.IsRequired();

				b.Property<DateTime>("CreatedOn")
					.ValueGeneratedOnAdd()
					.HasColumnType("DATETIME");

				b.Property<int?>("TherapistId");

				b.Property<string>("UserId");

				b.HasKey("Id");

				b.HasIndex("TherapistId");

				b.HasIndex("UserId");

				b.ToTable("Participants");
			});

			modelBuilder.Entity("Entities.Models.Site", b => {
				b.Property<int>("Id")
					.ValueGeneratedOnAdd();

				b.Property<string>("Name")
					.IsRequired();

				b.Property<string>("Shortname")
					.IsRequired();

				b.Property<int>("StudyId");

				b.HasKey("Id");

				b.HasIndex("StudyId");

				b.ToTable("Sites");
			});

			modelBuilder.Entity("Entities.Models.SiteParticipant", b => {
				b.Property<int>("SiteId");

				b.Property<int>("ParticipantId");

				b.HasKey("SiteId", "ParticipantId");

				b.HasIndex("ParticipantId");

				b.ToTable("SiteParticipants");
			});

			modelBuilder.Entity("Entities.Models.Study", b => {
				b.Property<int>("Id")
					.ValueGeneratedOnAdd();

				b.Property<string>("Name")
					.IsRequired();

				b.Property<int>("OrganizationId");

				b.Property<string>("Shortname")
					.IsRequired();

				b.Property<int>("SupervisorId");

				b.HasKey("Id");

				b.HasIndex("OrganizationId");

				b.HasIndex("SupervisorId");

				b.ToTable("Studies");
			});

			modelBuilder.Entity("Entities.Models.SubAdministrator", b => {
				b.Property<int>("Id")
					.ValueGeneratedOnAdd();

				b.Property<DateTime>("CreatedOn")
					.ValueGeneratedOnAdd()
					.HasColumnType("DATETIME");

				b.Property<string>("UserId");

				b.HasKey("Id");

				b.HasIndex("UserId");

				b.ToTable("SubAdministrators");
			});

			modelBuilder.Entity("Entities.Models.Supervisor", b => {
				b.Property<int>("Id")
					.ValueGeneratedOnAdd();

				b.Property<DateTime>("CreatedOn")
					.ValueGeneratedOnAdd()
					.HasColumnType("DATETIME");

				b.Property<string>("UserId");

				b.HasKey("Id");

				b.HasIndex("UserId");

				b.ToTable("Supervisors");
			});

			modelBuilder.Entity("Entities.Models.Therapist", b => {
				b.Property<int>("Id")
					.ValueGeneratedOnAdd();

				b.Property<DateTime>("CreatedOn")
					.ValueGeneratedOnAdd()
					.HasColumnType("DATETIME");

				b.Property<string>("UserId");

				b.HasKey("Id");

				b.HasIndex("UserId");

				b.ToTable("Therapists");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b => {
				b.Property<string>("Id")
					.ValueGeneratedOnAdd();

				b.Property<string>("ConcurrencyStamp")
					.IsConcurrencyToken();

				b.Property<string>("Name")
					.HasMaxLength(256);

				b.Property<string>("NormalizedName")
					.HasMaxLength(256);

				b.HasKey("Id");

				b.HasIndex("NormalizedName")
					.IsUnique()
					.HasName("RoleNameIndex");

				b.ToTable("AspNetRoles");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b => {
				b.Property<int>("Id")
					.ValueGeneratedOnAdd();

				b.Property<string>("ClaimType");

				b.Property<string>("ClaimValue");

				b.Property<string>("RoleId")
					.IsRequired();

				b.HasKey("Id");

				b.HasIndex("RoleId");

				b.ToTable("AspNetRoleClaims");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b => {
				b.Property<string>("Id")
					.ValueGeneratedOnAdd();

				b.Property<int>("AccessFailedCount");

				b.Property<string>("ConcurrencyStamp")
					.IsConcurrencyToken();

				b.Property<string>("Email")
					.HasMaxLength(256);

				b.Property<bool>("EmailConfirmed");

				b.Property<bool>("LockoutEnabled");

				b.Property<DateTimeOffset?>("LockoutEnd");

				b.Property<string>("NormalizedEmail")
					.HasMaxLength(256);

				b.Property<string>("NormalizedUserName")
					.HasMaxLength(256);

				b.Property<string>("PasswordHash");

				b.Property<string>("PhoneNumber");

				b.Property<bool>("PhoneNumberConfirmed");

				b.Property<string>("SecurityStamp");

				b.Property<bool>("TwoFactorEnabled");

				b.Property<string>("UserName")
					.HasMaxLength(256);

				b.HasKey("Id");

				b.HasIndex("NormalizedEmail")
					.HasName("EmailIndex");

				b.HasIndex("NormalizedUserName")
					.IsUnique()
					.HasName("UserNameIndex");

				b.ToTable("AspNetUsers");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b => {
				b.Property<int>("Id")
					.ValueGeneratedOnAdd();

				b.Property<string>("ClaimType");

				b.Property<string>("ClaimValue");

				b.Property<string>("UserId")
					.IsRequired();

				b.HasKey("Id");

				b.HasIndex("UserId");

				b.ToTable("AspNetUserClaims");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b => {
				b.Property<string>("LoginProvider");

				b.Property<string>("ProviderKey");

				b.Property<string>("ProviderDisplayName");

				b.Property<string>("UserId")
					.IsRequired();

				b.HasKey("LoginProvider", "ProviderKey");

				b.HasIndex("UserId");

				b.ToTable("AspNetUserLogins");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b => {
				b.Property<string>("UserId");

				b.Property<string>("RoleId");

				b.HasKey("UserId", "RoleId");

				b.HasIndex("RoleId");

				b.ToTable("AspNetUserRoles");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b => {
				b.Property<string>("UserId");

				b.Property<string>("LoginProvider");

				b.Property<string>("Name");

				b.Property<string>("Value");

				b.HasKey("UserId", "LoginProvider", "Name");

				b.ToTable("AspNetUserTokens");
			});

			modelBuilder.Entity("Entities.Models.Administrator", b => {
				b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "User")
					.WithMany()
					.HasForeignKey("UserId");
			});

			modelBuilder.Entity("Entities.Models.Organization", b => {
				b.HasOne("Entities.Models.SubAdministrator", "SubAdministrator")
					.WithMany()
					.HasForeignKey("SubAdministratorId")
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity("Entities.Models.Participant", b => {
				b.HasOne("Entities.Models.Therapist", "Therapist")
					.WithMany("Participants")
					.HasForeignKey("TherapistId");

				b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "User")
					.WithMany()
					.HasForeignKey("UserId");
			});

			modelBuilder.Entity("Entities.Models.Site", b => {
				b.HasOne("Entities.Models.Study", "Study")
					.WithMany("Sites")
					.HasForeignKey("StudyId")
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity("Entities.Models.SiteParticipant", b => {
				b.HasOne("Entities.Models.Participant", "Participant")
					.WithMany("SiteParticipants")
					.HasForeignKey("ParticipantId")
					.OnDelete(DeleteBehavior.Cascade);

				b.HasOne("Entities.Models.Site", "Site")
					.WithMany("SiteParticipants")
					.HasForeignKey("SiteId")
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity("Entities.Models.Study", b => {
				b.HasOne("Entities.Models.Organization", "Organization")
					.WithMany("Studies")
					.HasForeignKey("OrganizationId")
					.OnDelete(DeleteBehavior.Cascade);

				b.HasOne("Entities.Models.Supervisor", "Supervisor")
					.WithMany("Studies")
					.HasForeignKey("SupervisorId")
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity("Entities.Models.SubAdministrator", b => {
				b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "User")
					.WithMany()
					.HasForeignKey("UserId");
			});

			modelBuilder.Entity("Entities.Models.Supervisor", b => {
				b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "User")
					.WithMany()
					.HasForeignKey("UserId");
			});

			modelBuilder.Entity("Entities.Models.Therapist", b => {
				b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "User")
					.WithMany()
					.HasForeignKey("UserId");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b => {
				b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
					.WithMany()
					.HasForeignKey("RoleId")
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b => {
				b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b => {
				b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b => {
				b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
					.WithMany()
					.HasForeignKey("RoleId")
					.OnDelete(DeleteBehavior.Cascade);

				b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b => {
				b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade);
			});
#pragma warning restore 612, 618
		}
	}
}
