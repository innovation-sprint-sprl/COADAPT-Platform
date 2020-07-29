using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiModels {
	public class ParticipantRequest {
		[Required]
		public string UserName { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		public string Code { get; set; }
		public string StressfulEvents { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Sex { get; set; }
		public string Education { get; set; }
		public string Children { get; set; }
		public string Parents { get; set; }
		public DateTime DateOfFirstJob { get; set; }
		public DateTime DateOfCurrentJob { get; set; }
		public string JobType { get; set; }
		public string MedicalCondition { get; set; }
		public string Substances { get; set; }
		public string PhsychologicalCondition { get; set; }
		public string Region { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int TherapistId { get; set; }
		public IList<int> SiteIds { get; set; }
		public IList<int> GroupIds { get; set; }
	}
}
