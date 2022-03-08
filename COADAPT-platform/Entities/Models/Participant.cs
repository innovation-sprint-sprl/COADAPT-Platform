using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models {
	public class Participant : IUserDetails {

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column(TypeName = "DATETIME")]
		public DateTime CreatedOn { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime? DateOfBirth { get; set; }
        public string BirthPlace { get; set; }
        public string Language { get; set; }
        public string Gender { get; set; }
        public string Files { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime? DateOfFirstJob { get; set; }

        [Required]
		public string Code { get; set; }
		public string UserId { get; set; }
		public virtual IdentityUser User { get; set; }
		public int? TherapistId { get; set; }
		public virtual Therapist Therapist { get; set; }
		public ICollection<StudyParticipant> StudyParticipants { get; set; }
		public ICollection<PsychologicalReport> PsychologicalReports { get; set; }
		public ICollection<OuraActivity> OuraActivities { get; set; }
		public ICollection<OuraReadiness> OuraReadinesses { get; set; }
		public ICollection<OuraSleep> OuraSleeps { get; set; }
		public ICollection<PhysiologicalSignal> PhysiologicalSignals { get; set; }

	}
}
