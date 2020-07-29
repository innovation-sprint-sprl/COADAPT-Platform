using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models {
	public class Therapist : IUserDetails {

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column(TypeName = "DATETIME")]
		public DateTime CreatedOn { get; set; }

		public string UserId { get; set; }
		public virtual IdentityUser User { get; set; }

		public ICollection<Participant> Participants { get; set; }

	}
}
