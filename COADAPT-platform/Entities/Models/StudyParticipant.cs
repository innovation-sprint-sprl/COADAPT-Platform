using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class StudyParticipant
    {
        public int StudyId { get; set; }
        public virtual Study Study { get; set; }
        public int ParticipantId { get; set; }
        public virtual Participant Participant { get; set; }

        public int SiteId { get; set; }
        public virtual Site Site { get; set; }
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "DATETIME")]
        public DateTime? StartDate { get; set; }
        [Column(TypeName = "DATETIME")]
        public DateTime? EndDate { get; set; }

        public bool Abandoned { get; set; }
        public int DataCollectionTurn { get; set; }
        public string PlaceOfConsent { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime? DateOfCurrentJob { get; set; }
        public string JobType { get; set; }
        public string RegionOfResidence { get; set; }
        public string PlaceOfResidence { get; set; }
        public string MaritalStatus { get; set; }
        public int NumberOfChildren { get; set; }
        public string Education { get; set; }
    }
}