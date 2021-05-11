namespace ApiModels {
    public class StudyListResponse {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Shortname { get; set; }

        public string Organization { get; set; }

        public string Supervisor { get; set; }

        public int Sites { get; set; }

        public int Groups { get; set; }

        public int Participants { get; set; }
    }
}
