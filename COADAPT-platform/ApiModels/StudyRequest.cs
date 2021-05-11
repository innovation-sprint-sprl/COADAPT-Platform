namespace ApiModels {
	public class StudyRequest {
		public string Name { get; set; }
		public string Shortname { get; set; }
		public int OrganizationId { get; set; }
		public int SupervisorId { get; set; }
	}
}
