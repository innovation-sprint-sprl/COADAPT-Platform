using Entities.Models;
using ApiModels;

namespace Entities.Extensions {
    public static class OuraActivityExtensions {
        public static void Map(this OuraActivity dbOuraActivity, OuraActivity ouraActivity) {
            dbOuraActivity.ParticipantId = ouraActivity.ParticipantId;
            dbOuraActivity.SummaryDate = ouraActivity.SummaryDate;
            dbOuraActivity.TimezoneOffset = ouraActivity.TimezoneOffset;
            dbOuraActivity.DayStart = ouraActivity.DayStart;
            dbOuraActivity.DayEnd = ouraActivity.DayEnd;
            dbOuraActivity.Score = ouraActivity.Score;
            dbOuraActivity.ScoreRecoveryTime = ouraActivity.ScoreRecoveryTime;
            dbOuraActivity.ScoreStayActive = ouraActivity.ScoreStayActive;
            dbOuraActivity.ScoreTrainingFrequency = ouraActivity.ScoreTrainingFrequency;
            dbOuraActivity.ScoreTrainingVolume = ouraActivity.ScoreTrainingVolume;
            dbOuraActivity.ScoreMeetDailyTargets = ouraActivity.ScoreMeetDailyTargets;
            dbOuraActivity.ScoreMoveEveryHour = ouraActivity.ScoreMoveEveryHour;
            dbOuraActivity.DailyMovement = ouraActivity.DailyMovement;
            dbOuraActivity.NonWear = ouraActivity.NonWear;
            dbOuraActivity.Rest = ouraActivity.Rest;
            dbOuraActivity.Inactive = ouraActivity.Inactive;
            dbOuraActivity.InactivityAlerts = ouraActivity.InactivityAlerts;
            dbOuraActivity.Low = ouraActivity.Low;
            dbOuraActivity.Medium = ouraActivity.Medium;
            dbOuraActivity.High = ouraActivity.High;
            dbOuraActivity.Steps = ouraActivity.Steps;
            dbOuraActivity.CaloriesActive = ouraActivity.CaloriesActive;
            dbOuraActivity.CaloriesTotal = ouraActivity.CaloriesTotal;
            dbOuraActivity.Met1Min = ouraActivity.Met1Min;
            dbOuraActivity.MetMinHigh = ouraActivity.MetMinHigh;
            dbOuraActivity.MetMinInactive = ouraActivity.MetMinInactive;
            dbOuraActivity.MetMinLow = ouraActivity.MetMinLow;
            dbOuraActivity.MetMinMedium = ouraActivity.MetMinMedium;
            dbOuraActivity.MetMinMediumPlus = ouraActivity.MetMinMediumPlus;
            dbOuraActivity.AverageMet = ouraActivity.AverageMet;
            dbOuraActivity.Class5Min = ouraActivity.Class5Min;
            dbOuraActivity.TargetCalories = ouraActivity.TargetCalories;
            dbOuraActivity.TargetKm = ouraActivity.TargetKm;
            dbOuraActivity.TargetMiles = ouraActivity.TargetMiles;
            dbOuraActivity.ToTargetKm = ouraActivity.ToTargetKm;
            dbOuraActivity.ToTargetMiles = ouraActivity.ToTargetMiles;
        }

        public static void FromRequest(this OuraActivity ouraActivity, OuraActivityRequest request) {
            ouraActivity.ParticipantId = request.ParticipantId;
            ouraActivity.SummaryDate = request.SummaryDate;
            ouraActivity.TimezoneOffset = request.TimezoneOffset;
            ouraActivity.DayStart = request.DayStart;
            ouraActivity.DayEnd = request.DayEnd;
            ouraActivity.Score = request.Score;
            ouraActivity.ScoreRecoveryTime = request.ScoreRecoveryTime;
            ouraActivity.ScoreStayActive = request.ScoreStayActive;
            ouraActivity.ScoreTrainingFrequency = request.ScoreTrainingFrequency;
            ouraActivity.ScoreTrainingVolume = request.ScoreTrainingVolume;
            ouraActivity.ScoreMeetDailyTargets = request.ScoreMeetDailyTargets;
            ouraActivity.ScoreMoveEveryHour = request.ScoreMoveEveryHour;
            ouraActivity.DailyMovement = request.DailyMovement;
            ouraActivity.NonWear = request.NonWear;
            ouraActivity.Rest = request.Rest;
            ouraActivity.Inactive = request.Inactive;
            ouraActivity.InactivityAlerts = request.InactivityAlerts;
            ouraActivity.Low = request.Low;
            ouraActivity.Medium = request.Medium;
            ouraActivity.High = request.High;
            ouraActivity.Steps = request.Steps;
            ouraActivity.CaloriesActive = request.CaloriesActive;
            ouraActivity.CaloriesTotal = request.CaloriesTotal;
            ouraActivity.Met1Min = string.Join(",", request.Met1Min);
            ouraActivity.MetMinHigh = request.MetMinHigh;
            ouraActivity.MetMinInactive = request.MetMinInactive;
            ouraActivity.MetMinLow = request.MetMinLow;
            ouraActivity.MetMinMedium = request.MetMinMedium;
            ouraActivity.MetMinMediumPlus = request.MetMinMediumPlus;
            ouraActivity.AverageMet = request.AverageMet;
            ouraActivity.Class5Min = request.Class5Min;
            ouraActivity.TargetCalories = request.TargetCalories;
            ouraActivity.TargetKm = request.TargetKm;
            ouraActivity.TargetMiles = request.TargetMiles;
            ouraActivity.ToTargetKm = request.ToTargetKm;
            ouraActivity.ToTargetMiles = request.ToTargetMiles;
        }
    }
}