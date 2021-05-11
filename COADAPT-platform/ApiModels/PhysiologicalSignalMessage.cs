namespace ApiModels {
    public class PhysiologicalSignalMessage {
        public long Timestamp { get; set; }
        public string Type { get; set; }
        public float Value { get; set; }
        public float? Accuracy { get; set; }
    }
}
