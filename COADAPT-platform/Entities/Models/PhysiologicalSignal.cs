using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models {
    public class PhysiologicalSignal {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ParticipantId { get; set; }
        public virtual Participant Participant { get; set; }

        [Column(TypeName = "DATETIME(3)")]
        public DateTime Timestamp { get; set; }

        public string Type { get; set; }
        public float Value { get; set; }
        public float? Accuracy { get; set; }
    }
}
