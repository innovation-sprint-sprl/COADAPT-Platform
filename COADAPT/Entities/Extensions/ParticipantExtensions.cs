using Entities.Models;

namespace Entities.Extensions {
	public static class ParticipantExtensions {
		public static void Map(this Participant dbParticipant, Participant participant) {
			dbParticipant.UserId = participant.UserId;
			dbParticipant.TherapistId = participant.TherapistId;
			dbParticipant.Code = participant.Code;
            dbParticipant.StressfulEvents = participant.StressfulEvents;
            dbParticipant.DateOfBirth = participant.DateOfBirth;
            dbParticipant.Sex = participant.Sex;
            dbParticipant.Education = participant.Education;
            dbParticipant.Children = participant.Children;
            dbParticipant.Parents = participant.Parents;
            dbParticipant.DateOfFirstJob = participant.DateOfFirstJob;
            dbParticipant.DateOfCurrentJob = participant.DateOfCurrentJob;
            dbParticipant.JobType = participant.JobType;
            dbParticipant.MedicalCondition = participant.MedicalCondition;
            dbParticipant.Substances = participant.Substances;
            dbParticipant.PhsychologicalCondition = participant.PhsychologicalCondition;
            dbParticipant.Region = participant.Region;
            dbParticipant.StartDate = participant.StartDate;
            dbParticipant.EndDate = participant.EndDate;
        }
    }
}
