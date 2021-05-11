using Entities.Models;
using ApiModels;

namespace Entities.Extensions {
    public static class OuraSleepExtensions {
        public static void Map(this OuraSleep dbOuraSleep, OuraSleep ouraSleep) {
            dbOuraSleep.ParticipantId = ouraSleep.ParticipantId;
            dbOuraSleep.SummaryDate = ouraSleep.SummaryDate;
            dbOuraSleep.PeriodId = ouraSleep.PeriodId;
            dbOuraSleep.TimezoneOffset = ouraSleep.TimezoneOffset;
            dbOuraSleep.BedtimeStart = ouraSleep.BedtimeStart;
            dbOuraSleep.BedtimeEnd = ouraSleep.BedtimeEnd;
            dbOuraSleep.Score = ouraSleep.Score;
            dbOuraSleep.ScoreTotal = ouraSleep.ScoreTotal;
            dbOuraSleep.ScoreDisturbances = ouraSleep.ScoreDisturbances;
            dbOuraSleep.ScoreAlignment = ouraSleep.ScoreAlignment;
            dbOuraSleep.ScoreDeep = ouraSleep.ScoreDeep;
            dbOuraSleep.ScoreEfficiency = ouraSleep.ScoreEfficiency;
            dbOuraSleep.ScoreLatency = ouraSleep.ScoreLatency;
            dbOuraSleep.ScoreRem = ouraSleep.ScoreRem;
            dbOuraSleep.Total = ouraSleep.Total;
            dbOuraSleep.Duration = ouraSleep.Duration;
            dbOuraSleep.Awake = ouraSleep.Awake;
            dbOuraSleep.Light = ouraSleep.Light;
            dbOuraSleep.Rem = ouraSleep.Rem;
            dbOuraSleep.Deep = ouraSleep.Deep;
            dbOuraSleep.OnsetLatency = ouraSleep.OnsetLatency;
            dbOuraSleep.Restless = ouraSleep.Restless;
            dbOuraSleep.Efficiency = ouraSleep.Efficiency;
            dbOuraSleep.MidpointTime = ouraSleep.MidpointTime;
            dbOuraSleep.HrLowest = ouraSleep.HrLowest;
            dbOuraSleep.HrAverage = ouraSleep.HrAverage;
            dbOuraSleep.RmsSd = ouraSleep.RmsSd;
            dbOuraSleep.BreathAverage = ouraSleep.BreathAverage;
            dbOuraSleep.TemperatureDelta = ouraSleep.TemperatureDelta;
            dbOuraSleep.Hypnogram5Min = ouraSleep.Hypnogram5Min;
            dbOuraSleep.Hr5Min = ouraSleep.Hr5Min;
            dbOuraSleep.RmsSd5Min = ouraSleep.RmsSd5Min;
        }

        public static void FromRequest(this OuraSleep ouraSleep, OuraSleepRequest request) {
            ouraSleep.ParticipantId = request.ParticipantId;
            ouraSleep.SummaryDate = request.SummaryDate;
            ouraSleep.PeriodId = request.PeriodId;
            ouraSleep.TimezoneOffset = request.TimezoneOffset;
            ouraSleep.BedtimeStart = request.BedtimeStart;
            ouraSleep.BedtimeEnd = request.BedtimeEnd;
            ouraSleep.Score = request.Score;
            ouraSleep.ScoreTotal = request.ScoreTotal;
            ouraSleep.ScoreDisturbances = request.ScoreDisturbances;
            ouraSleep.ScoreAlignment = request.ScoreAlignment;
            ouraSleep.ScoreDeep = request.ScoreDeep;
            ouraSleep.ScoreEfficiency = request.ScoreEfficiency;
            ouraSleep.ScoreLatency = request.ScoreLatency;
            ouraSleep.ScoreRem = request.ScoreRem;
            ouraSleep.Total = request.Total;
            ouraSleep.Duration = request.Duration;
            ouraSleep.Awake = request.Awake;
            ouraSleep.Light = request.Light;
            ouraSleep.Rem = request.Rem;
            ouraSleep.Deep = request.Deep;
            ouraSleep.OnsetLatency = request.OnsetLatency;
            ouraSleep.Restless = request.Restless;
            ouraSleep.Efficiency = request.Efficiency;
            ouraSleep.MidpointTime = request.MidpointTime;
            ouraSleep.HrLowest = request.HrLowest;
            ouraSleep.HrAverage = request.HrAverage;
            ouraSleep.RmsSd = request.RmsSd;
            ouraSleep.BreathAverage = request.BreathAverage;
            ouraSleep.TemperatureDelta = request.TemperatureDelta;
            ouraSleep.Hypnogram5Min = request.Hypnogram5Min;
            ouraSleep.Hr5Min = string.Join(",", request.Hr5Min);
            ouraSleep.RmsSd5Min = string.Join(",", request.RmsSd5Min);
        }
    }
}