using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Entities {

	public class COADAPTContext : IdentityDbContext {

		public COADAPTContext(DbContextOptions<COADAPTContext> options) : base(options) { }

		public DbSet<Group> Groups { get; set; }
		public DbSet<Site> Sites { get; set; }
		public DbSet<Study> Studies { get; set; }
		public DbSet<Organization> Organizations { get; set; }
		public DbSet<Administrator> Administrators { get; set; }
		public DbSet<SubAdministrator> SubAdministrators { get; set; }
		public DbSet<Supervisor> Supervisors { get; set; }
		public DbSet<Therapist> Therapists { get; set; }
		public DbSet<Participant> Participants { get; set; }
		public DbSet<StudyParticipant> StudyParticipants { get; set; }
		public DbSet<PsychologicalReport> PsychologicalReports { get; set; }
		public DbSet<PhysiologicalSignal> PhysiologicalSignals { get; set; }
		public DbSet<OuraSleep> OuraSleeps { get; set; }
		public DbSet<OuraActivity> OuraActivities { get; set; }
		public DbSet<OuraReadiness> OuraReadinesses { get; set; }
		public DbSet<AppUsageLog> AppUsageLogs { get; set; }

		public DbSet<UserAccessToken> UserAccessToken { get; set; }

		protected override void OnModelCreating(ModelBuilder builder) {
			base.OnModelCreating(builder);

			builder.Entity<StudyParticipant>().HasKey(sp => new { sp.StudyId, sp.ParticipantId });
			builder.Entity<UserAccessToken>().HasKey(x => new { x.UserId, x.RefreshToken });
			
		}

	}

}