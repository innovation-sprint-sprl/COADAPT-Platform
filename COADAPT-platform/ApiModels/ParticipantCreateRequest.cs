using System;
using System.ComponentModel.DataAnnotations;

namespace ApiModels {
	public class ParticipantCreateRequest {
		[Required]
		public string Code { get; set; }
		[Required]
		public string UserName { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		public int SiteId { get; set; }
		[Required]
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
