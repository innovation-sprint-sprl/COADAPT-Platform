using System;
using System.Collections.Generic;

namespace ApiModels
{
	public class SupervisorListResponse {

		public int Id { get; set; }

		public string UserName { get; set; }

		public DateTime CreatedOn { get; set; }

        public List<string> Organizations { get; set; }

        public List<string> Studies { get; set; }

        public int Participants { get; set; }

	}
}
