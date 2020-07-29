using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models {

	public class Organization : ILocationDetails {

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Shortname { get; set; }

		public ICollection<Study> Studies { get; set; }

		public int SubAdministratorId { get; set; }
		public virtual SubAdministrator SubAdministrator { get; set; }

	}

}
