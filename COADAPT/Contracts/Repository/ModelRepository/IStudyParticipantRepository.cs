using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repository.ModelRepository {

	public interface IStudyParticipantRepository {

		Task<IEnumerable<StudyParticipant>> StudyParticipantsByStudy(int studyId);
		Task<IEnumerable<StudyParticipant>> StudyParticipantsByParticipant(int participantId);
        Task<IEnumerable<StudyParticipant>> StudyParticipantsByStudyAndParticipant(int studyId, int participantId);
        Task<IEnumerable<StudyParticipant>> StudyParticipantsBySite(int siteId);
        Task<IEnumerable<StudyParticipant>> StudyParticipantsByGroup(int groupId);
        Task<StudyParticipant> ActiveStudyParticipantByParticipantAndStudy(int participantId, int studyId);
		Task<IEnumerable<StudyParticipant>> GetAllStudyParticipantsAsync();
		void CreateStudyParticipant(StudyParticipant studyParticipant);
		void DeleteStudyParticipant(StudyParticipant studyParticipant);
        void UpdateStudyParticipant(StudyParticipant dbStudyParticipant, StudyParticipant studyParticipant);

    }
}
