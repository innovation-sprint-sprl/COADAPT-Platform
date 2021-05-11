using Entities.Models;
using ApiModels;

namespace Entities.Extensions {
    public static class OuraReadinessExtensions {
        public static void Map(this OuraReadiness dbOuraReadiness, OuraReadiness ouraReadiness) {
            dbOuraReadiness.ParticipantId = ouraReadiness.ParticipantId;
            dbOuraReadiness.SummaryDate = ouraReadiness.SummaryDate;
            dbOuraReadiness.PeriodId = ouraReadiness.PeriodId;
            dbOuraReadiness.Score = ouraReadiness.Score;
            dbOuraReadiness.ScoreTemperature = ouraReadiness.ScoreTemperature;
            dbOuraReadiness.ScoreActivityBalance = ouraReadiness.ScoreActivityBalance;
            dbOuraReadiness.ScoreHrvBalance = ouraReadiness.ScoreHrvBalance;
            dbOuraReadiness.ScorePreviousDay = ouraReadiness.ScorePreviousDay;
            dbOuraReadiness.ScorePreviousNight = ouraReadiness.ScorePreviousNight;
            dbOuraReadiness.ScoreRecoveryIndex = ouraReadiness.ScoreRecoveryIndex;
            dbOuraReadiness.ScoreRestingHr = ouraReadiness.ScoreRestingHr;
            dbOuraReadiness.ScoreSleepBalance = ouraReadiness.ScoreSleepBalance;
        }

        public static void FromRequest(this OuraReadiness ouraReadiness, OuraReadinessRequest request) {
            ouraReadiness.ParticipantId = request.ParticipantId;
            ouraReadiness.SummaryDate = request.SummaryDate;
            ouraReadiness.PeriodId = request.PeriodId;
            ouraReadiness.Score = request.Score;
            ouraReadiness.ScoreTemperature = request.ScoreTemperature;
            ouraReadiness.ScoreActivityBalance = request.ScoreActivityBalance;
            ouraReadiness.ScoreHrvBalance = request.ScoreHrvBalance;
            ouraReadiness.ScorePreviousDay = request.ScorePreviousDay;
            ouraReadiness.ScorePreviousNight = request.ScorePreviousNight;
            ouraReadiness.ScoreRecoveryIndex = request.ScoreRecoveryIndex;
            ouraReadiness.ScoreRestingHr = request.ScoreRestingHr;
            ouraReadiness.ScoreSleepBalance = request.ScoreSleepBalance;
        }
    }
}