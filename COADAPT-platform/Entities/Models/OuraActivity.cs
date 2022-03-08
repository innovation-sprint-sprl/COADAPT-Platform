using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models {
    public class OuraActivity {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ParticipantId { get; set; }
        public virtual Participant Participant { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime SummaryDate { get; set; }
        
        public int TimezoneOffset { get; set; }
        [Column(TypeName = "DATETIME")]
        public DateTime DayStart { get; set; }
        [Column(TypeName = "DATETIME")]
        public DateTime DayEnd { get; set; }
        public int Score { get; set; }
        public int ScoreStayActive { get; set; }
        public int ScoreMoveEveryHour { get; set; }
        public int ScoreMeetDailyTargets { get; set; }
        public int ScoreTrainingFrequency { get; set; }
        public int ScoreTrainingVolume { get; set; }
        public int ScoreRecoveryTime { get; set; }
        public int DailyMovement { get; set; }
        public int NonWear { get; set; }
        public int Rest { get; set; }
        public int Inactive { get; set; }
        public int InactivityAlerts { get; set; }
        public int Low { get; set; }
        public int Medium { get; set; }
        public int High { get; set; }
        public int Steps { get; set; }
        public int CaloriesTotal { get; set; }
        public int CaloriesActive { get; set; }
        public int MetMinInactive { get; set; }
        public int MetMinLow { get; set; }
        public int MetMinMedium { get; set; }
        public int MetMinMediumPlus { get; set; }
        public int MetMinHigh { get; set; }
        public float AverageMet { get; set; }
        public string Class5Min { get; set; }
        public string Met1Min { get; set; }
        public float TargetCalories { get; set; }
        public float TargetKm { get; set; }
        public float TargetMiles { get; set; }
        public float ToTargetKm { get; set; }
        public float ToTargetMiles { get; set; }
    }
}