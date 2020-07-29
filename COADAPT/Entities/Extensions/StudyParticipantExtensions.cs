using Entities.Models;

namespace Entities.Extensions {
    public static class StudyParticipantExtensions {
        public static void Map(this StudyParticipant dbStudyParticipant, StudyParticipant studyParticipant) {
            dbStudyParticipant.ParticipantId = studyParticipant.ParticipantId;
            dbStudyParticipant.StudyId = studyParticipant.StudyId;
            dbStudyParticipant.SiteId = studyParticipant.SiteId;
            dbStudyParticipant.GroupId = studyParticipant.GroupId;
            dbStudyParticipant.Participant = studyParticipant.Participant;
            dbStudyParticipant.Study = studyParticipant.Study;
            dbStudyParticipant.Site = studyParticipant.Site;
            dbStudyParticipant.Group = studyParticipant.Group;
            dbStudyParticipant.StartDate = studyParticipant.StartDate;
            dbStudyParticipant.EndDate = studyParticipant.EndDate;
        }
    }
}
