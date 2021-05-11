using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiModels {
	public class ParticipantUpdateRequest {
		
		public string UserName { get; set; }
		public string Password { get; set; }
		public int SiteId { get; set; }
		public int GroupId { get; set; }
		public string Gender { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public DateTime? DateOfFirstJob { get; set; }
		public int? TherapistId { get; set; }
		public string Education { get; set; }
		public string Region { get; set; }
		public string MaritalStatus { get; set; }
		public DateTime? DateOfCurrentJob { get; set; }
		public string JobType { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}
}

