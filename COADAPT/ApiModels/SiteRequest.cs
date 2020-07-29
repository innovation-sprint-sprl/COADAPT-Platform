using System.Collections.Generic;

namespace ApiModels {
	public class SiteRequest {
		public string Name { get; set; }
		public string Shortname { get; set; }
		public int StudyId { get; set; }
		//public IList<int> ParticipantIds { get; set; }
	}
}
