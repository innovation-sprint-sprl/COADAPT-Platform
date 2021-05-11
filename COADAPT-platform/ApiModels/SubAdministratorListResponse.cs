using System;

namespace ApiModels {

	public class SubAdministratorListResponse {

		public int Id { get; set; }

		public string UserName { get; set; }

		public DateTime CreatedOn { get; set; }

		public string Organization { get; set; }

		public int Participants { get; set; }

	}

}
