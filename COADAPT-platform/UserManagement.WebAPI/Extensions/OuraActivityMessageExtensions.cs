using ApiModels;
using Entities.Models;

namespace UserManagement.WebAPI.Extensions {
	public static class OuraActivityMessageExtensions {
		public static OuraActivity ToOuraActivity(this OuraActivityMessage message, int participantId) {
			var activity = new OuraActivity {
				ParticipantId = participantId,
				SummaryDate = message.SummaryDate,
				TimezoneOffset = message.TimezoneOffset,
				DayStart = message.DayStart,
				DayEnd = message.DayEnd,
				Score = message.Score,
				ScoreStayActive = message.ScoreStayActive,
				ScoreMoveEveryHour = message.ScoreMoveEveryHour,
				ScoreMeetDailyTargets = message.ScoreMeetDailyTargets,
				ScoreTrainingFrequency = message.ScoreTrainingFrequency,
				ScoreTrainingVolume = message.ScoreTrainingVolume,
				ScoreRecoveryTime = message.ScoreRecoveryTime,
				DailyMovement = message.DailyMovement,
				NonWear = message.NonWear,
				Rest = message.Rest,
				Inactive = message.Inactive,
				InactivityAlerts = message.InactivityAlerts,
				Low = message.Low,
				Medium = message.Medium,
				High = message.High,
				Steps = message.Steps,
				CaloriesTotal = message.CaloriesTotal,
				CaloriesActive = message.CaloriesActive,
				MetMinInactive = message.MetMinInactive,
				MetMinLow = message.MetMinLow,
				MetMinMedium = message.MetMinMedium,
				MetMinMediumPlus = message.MetMinMediumPlus,
				MetMinHigh = message.MetMinHigh,
				AverageMet = message.AverageMet,
				Class5Min = message.Class5Min,
				TargetCalories = message.TargetCalories,
				TargetKm = message.TargetKm,
				TargetMiles = message.TargetMiles,
				ToTargetKm = message.ToTargetKm,
				ToTargetMiles = message.ToTargetMiles,
				Met1Min = string.Join(",", message.Met1Min)
			};
			return activity;
		}
	}
}
