using System;
using ApiModels;
using Entities.Models;

namespace UserManagement.WebAPI.Extensions {
	public static class PhysiologicalSignalMessageExtensions {
		public static PhysiologicalSignal ToPhysiologicalSignal(this PhysiologicalSignalMessage message, int participantId) {
			var signal = new PhysiologicalSignal {
				ParticipantId = participantId,
				Type = message.Type,
				Value = message.Value,
				Accuracy = message.Accuracy,
				Timestamp = new DateTime(1970,1,1,0,0,0,0)
			};
			signal.Timestamp = signal.Timestamp.AddMilliseconds(message.Timestamp);
			return signal;
		}
	}
}
