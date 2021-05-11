namespace ApiModels
{
    public class GroupListResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Shortname { get; set; }

        public string Organization { get; set; }

        public int StudyId { get; set; }

        public string Study { get; set; }

        public string StudyShortname { get; set; }

        public int Participants { get; set; }
    }
}
