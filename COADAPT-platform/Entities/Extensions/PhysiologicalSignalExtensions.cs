using Entities.Models;
using ApiModels;

namespace Entities.Extensions {
    public static class PhysiologicalSignalExtensions {
        public static void Map(this PhysiologicalSignal dbPhysiologicalSignal, PhysiologicalSignal physiologicalSignal) {
            dbPhysiologicalSignal.ParticipantId = physiologicalSignal.ParticipantId;
            dbPhysiologicalSignal.Timestamp = physiologicalSignal.Timestamp;
            dbPhysiologicalSignal.Type = physiologicalSignal.Type;
            dbPhysiologicalSignal.Value = physiologicalSignal.Value;
            dbPhysiologicalSignal.Accuracy = physiologicalSignal.Accuracy;
        }

        public static void FromRequest(this PhysiologicalSignal physiologicalSignal, PhysiologicalSignalRequest request) {
            physiologicalSignal.ParticipantId = request.ParticipantId;
            physiologicalSignal.Timestamp = request.Timestamp;
            physiologicalSignal.Type = request.Type;
            physiologicalSignal.Value = request.Value;
            physiologicalSignal.Accuracy = request.Accuracy;
        }
    }
}