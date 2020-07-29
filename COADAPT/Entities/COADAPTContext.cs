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
        public DbSet<PersonalInformation> PersonalInformations { get; set; }
        public DbSet<AppUsageLog> AppUsageLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
			base.OnModelCreating(builder);
			builder.Entity<StudyParticipant>()
				.HasKey(sp => new {sp.StudyId, sp.ParticipantId});
			//builder.Entity<SiteParticipant>()
			//    .HasOne(sp => sp.Site)
			//    .WithMany(s => s.SiteParticipants)
			//    .HasForeignKey(sp => sp.SiteId);
			//builder.Entity<SiteParticipant>()
			//    .HasOne(sp => sp.Participant)
			//    .WithMany(p => p.SiteParticipants)
			//    .HasForeignKey(sp => sp.ParticipantId);
			/*            builder.Entity<SubAdministrator>()
                            .HasOne(subadmin => subadmin.Organization)
                            .WithOne(org => org.SubAdministrator)
                            .HasForeignKey<Organization>(org => org.SubAdministratorForeignKey);
            */
        }

	}

}
