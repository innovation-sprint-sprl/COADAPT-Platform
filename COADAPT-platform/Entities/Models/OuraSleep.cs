using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models {
    public class OuraSleep {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ParticipantId { get; set; }
        public virtual Participant Participant { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime SummaryDate { get; set; }

        public int PeriodId { get; set; }
        public int TimezoneOffset { get; set; }
        [Column(TypeName = "DATETIME")]
        public DateTime BedtimeStart { get; set; }
        [Column(TypeName = "DATETIME")]
        public DateTime BedtimeEnd { get; set; }
        public int Score { get; set; }
        public int ScoreTotal { get; set; }
        public int ScoreDisturbances { get; set; }
        public int ScoreEfficiency { get; set; }
        public int ScoreLatency { get; set; }
        public int ScoreRem { get; set; }
        public int ScoreDeep { get; set; }
        public int ScoreAlignment { get; set; }
        public int Total { get; set; }
        public int Duration { get; set; }
        public int Awake { get; set; }
        public int Light { get; set; }
        public int Rem { get; set; }
        public int Deep { get; set; }
        public int OnsetLatency { get; set; }
        public int Restless { get; set; }
        public int Efficiency { get; set; }
        public int MidpointTime { get; set; }
        public int HrLowest { get; set; }
        public int HrAverage { get; set; }
        public int RmsSd { get; set; }
        public int BreathAverage { get; set; }
        public int TemperatureDelta { get; set; }
        public string Hypnogram5Min { get; set; }
        public string Hr5Min { get; set; }
        public string RmsSd5Min { get; set; }

    }
}