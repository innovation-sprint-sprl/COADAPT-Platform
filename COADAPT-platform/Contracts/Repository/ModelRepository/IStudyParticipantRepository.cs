using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repository.ModelRepository {

	public interface IStudyParticipantRepository : IRepositoryBase<StudyParticipant> {

		Task<IEnumerable<StudyParticipant>> StudyParticipantsByStudy(int studyId, bool? abandoned = null);
		Task<IEnumerable<StudyParticipant>> StudyParticipantsByParticipant(int participantId);
        Task<IEnumerable<StudyParticipant>> StudyParticipantsByStudyAndParticipant(int studyId, int participantId);
        Task<IEnumerable<StudyParticipant>> StudyParticipantsBySite(int siteId, bool? abandoned = null);
        Task<IEnumerable<StudyParticipant>> StudyParticipantsByGroup(int groupId, bool? abandoned = null);
        Task<StudyParticipant> StudyParticipantByParticipantAndStudy(int participantId, int studyId);
		Task<IEnumerable<StudyParticipant>> GetAllStudyParticipantsAsync();
		void CreateStudyParticipant(StudyParticipant studyParticipant);
		void DeleteStudyParticipant(StudyParticipant studyParticipant);
        void UpdateStudyParticipant(StudyParticipant dbStudyParticipant, StudyParticipant studyParticipant);

    }
}
