using System;
using System.Collections.Generic;

namespace ApiModels {

	public class ParticipantListResponse{

		public int Id { get; set; }

		public string Code { get; set; }

		public DateTime CreatedOn { get; set; }

        public string Therapist { get; set; }

        public List<string> Organizations { get; set; }

        public List<string> Studies { get; set; }

        public List<string> Sites { get; set; }

        public List<string> Groups { get; set; }

	}

}
