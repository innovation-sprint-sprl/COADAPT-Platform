using System;
using System.ComponentModel.DataAnnotations;

namespace ApiModels {
    public class PhysiologicalSignalRequest {
        [Required]
        public int ParticipantId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type { get; set; }
        public float Value { get; set; }
        public float? Accuracy { get; set; }
    }
}