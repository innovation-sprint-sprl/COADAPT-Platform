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

        public string StressfulEvents { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime DateOfBirth { get; set; }

        public string Sex { get; set; }

        public string Education { get; set; }

        public string Children { get; set; }

        public string Parents { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime DateOfFirstJob { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime DateOfCurrentJob { get; set; }

        public string JobType { get; set; }

        public string MedicalCondition { get; set; }

        public string Substances { get; set; }

        public string PhsychologicalCondition { get; set; }

        public string Region { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime EndDate { get; set; }

        [Required]
		public string Code { get; set; }

		public string UserId { get; set; }
		public virtual IdentityUser User { get; set; }

		public int? TherapistId { get; set; }
		public virtual Therapist Therapist { get; set; }

		//public ICollection<SiteParticipant> SiteParticipants { get; set; }
		public ICollection<StudyParticipant> StudyParticipants { get; set; }

	}
}
