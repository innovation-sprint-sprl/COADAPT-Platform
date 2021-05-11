using ApiModels;
using Entities.Models;

namespace UserManagement.WebAPI.Extensions {
	public static class OuraSleepMessageExtensions {
		public static OuraSleep ToOuraSleep(this OuraSleepMessage message, int participantId) {
			var sleep = new OuraSleep {
				ParticipantId = participantId,
				SummaryDate = message.SummaryDate,
				TimezoneOffset = message.TimezoneOffset,
				PeriodId = message.PeriodId,
				BedtimeStart = message.BedtimeStart,
				BedtimeEnd = message.BedtimeEnd,
				Score = message.Score,
				ScoreTotal = message.ScoreTotal,
				ScoreDisturbances = message.ScoreDisturbances,
				ScoreEfficiency = message.ScoreEfficiency,
				ScoreLatency = message.ScoreLatency,
				ScoreRem = message.ScoreRem,
				ScoreDeep = message.ScoreDeep,
				ScoreAlignment = message.ScoreAlignment,
				Total = message.Total,
				Duration = message.Duration,
				Awake = message.Awake,
				Light = message.Light,
				Rem = message.Rem,
				Deep = message.Deep,
				OnsetLatency = message.OnsetLatency,
				Restless = message.Restless,
				Efficiency = message.Efficiency,
				MidpointTime = message.MidpointTime,
				HrLowest = message.HrLowest,
				HrAverage = message.HrAverage,
				RmsSd = message.RmsSd,
				BreathAverage = message.BreathAverage,
				TemperatureDelta = message.TemperatureDelta,
				Hypnogram5Min = message.Hypnogram5Min,
				Hr5Min = string.Join(",", message.Hr5Min),
				RmsSd5Min = string.Join(",", message.RmsSd5Min)
			};
			return sleep;
		}
	}
}
