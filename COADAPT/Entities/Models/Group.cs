using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models {
    public class Group : ILocationDetails {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Shortname { get; set; }

        public int StudyId { get; set; }
        public virtual Study Study { get; set; }

        //public ICollection<SiteParticipant> SiteParticipants { get; set; }
    }
}