using Entities.Models;

namespace Entities.Extensions {
	public static class ParticipantExtensions {
		public static void Map(this Participant dbParticipant, Participant participant) {
			dbParticipant.UserId = participant.UserId;
			dbParticipant.TherapistId = participant.TherapistId;
			dbParticipant.Code = participant.Code;
            dbParticipant.DateOfBirth = participant.DateOfBirth;
            dbParticipant.BirthPlace = participant.BirthPlace;
            dbParticipant.Language = participant.Language;
            dbParticipant.Gender = participant.Gender;
            dbParticipant.Files = participant.Files;
            dbParticipant.DateOfFirstJob = participant.DateOfFirstJob;
        }
    }
}
