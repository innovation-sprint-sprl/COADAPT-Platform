using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiModels {
    public class OuraActivityRequest {

        [Required]
        public int ParticipantId { get; set; }
        public DateTime SummaryDate { get; set; }
        public int TimezoneOffset { get; set; }
        public DateTime DayStart { get; set; }
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
        public int AverageMet { get; set; }
        public string Class5Min { get; set; }
        public List<double> Met1Min { get; set; }
        public int TargetCalories { get; set; }
        public int TargetKm { get; set; }
        public int TargetMiles { get; set; }
        public int ToTargetKm { get; set; }
        public int ToTargetMiles { get; set; }
    }
}
