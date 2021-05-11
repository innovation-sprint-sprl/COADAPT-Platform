using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models {

	public class Study : ILocationDetails {

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Shortname { get; set; }

		public int OrganizationId { get; set; }
		public virtual Organization Organization { get; set; }

		public ICollection<Site> Sites { get; set; }
		public ICollection<Group> Groups { get; set; }

		public int SupervisorId { get; set; }
		public virtual Supervisor Supervisor { get; set; }

		public ICollection<StudyParticipant> StudyParticipants { get; set; }
	}

}
