using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models {
    public class OuraReadiness {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ParticipantId { get; set; }
        public virtual Participant Participant { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime SummaryDate { get; set; }

        public int PeriodId { get; set; }
        public int Score { get; set; }
        public int ScorePreviousNight { get; set; }
        public int ScoreSleepBalance { get; set; }
        public int ScorePreviousDay { get; set; }
        public int ScoreActivityBalance { get; set; }
        public int ScoreRestingHr { get; set; }
        public int ScoreHrvBalance { get; set; }
        public int ScoreRecoveryIndex { get; set; }
        public int ScoreTemperature { get; set; }
    }
}
