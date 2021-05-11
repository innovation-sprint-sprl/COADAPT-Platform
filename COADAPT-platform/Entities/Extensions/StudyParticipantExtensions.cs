using Entities.Models;

namespace Entities.Extensions {
    public static class StudyParticipantExtensions {
        public static void Map(this StudyParticipant dbStudyParticipant, StudyParticipant studyParticipant) {
            dbStudyParticipant.ParticipantId = studyParticipant.ParticipantId;
            dbStudyParticipant.StudyId = studyParticipant.StudyId;
            dbStudyParticipant.SiteId = studyParticipant.SiteId;
            dbStudyParticipant.GroupId = studyParticipant.GroupId;
            dbStudyParticipant.StartDate = studyParticipant.StartDate;
            dbStudyParticipant.EndDate = studyParticipant.EndDate;
            dbStudyParticipant.Abandoned = studyParticipant.Abandoned;
            dbStudyParticipant.DataCollectionTurn = studyParticipant.DataCollectionTurn;
            dbStudyParticipant.PlaceOfConsent = studyParticipant.PlaceOfConsent;
            dbStudyParticipant.RegionOfResidence = studyParticipant.RegionOfResidence;
            dbStudyParticipant.PlaceOfResidence = studyParticipant.PlaceOfResidence;
            dbStudyParticipant.MaritalStatus = studyParticipant.MaritalStatus;
            dbStudyParticipant.NumberOfChildren = studyParticipant.NumberOfChildren;
            dbStudyParticipant.DateOfCurrentJob = studyParticipant.DateOfCurrentJob;
            dbStudyParticipant.JobType = studyParticipant.JobType;
            dbStudyParticipant.Education = studyParticipant.Education;
        }
    }
}
