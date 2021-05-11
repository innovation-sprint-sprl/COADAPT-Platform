using System;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models {
    public class OuraReadinessRequest {

        [Required]
        public int ParticipantId { get; set; }
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

