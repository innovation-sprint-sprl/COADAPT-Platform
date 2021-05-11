using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.ModelRepository {

	class StudyParticipantRepository : RepositoryBase<StudyParticipant>, IStudyParticipantRepository {

		public StudyParticipantRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

		public void CreateStudyParticipant(StudyParticipant studyParticipant) {
			Create(studyParticipant);
		}

		public void DeleteStudyParticipant(StudyParticipant studyParticipant) {
			Delete(studyParticipant);
		}

		public async Task<StudyParticipant> StudyParticipantByParticipantAndStudy(int participantId, int studyId) {
			return await FindByCondition(sp => sp.StudyId.Equals(studyId) &&
			                                   sp.ParticipantId.Equals(participantId))
				.DefaultIfEmpty(new StudyParticipant())
				.SingleAsync();
		}

		public async Task<IEnumerable<StudyParticipant>> GetAllStudyParticipantsAsync() {
			return await FindAll().ToListAsync();
		}

		public async Task<IEnumerable<StudyParticipant>> StudyParticipantsByStudyAndParticipant(int studyId, int participantId) {
			return await FindByCondition(sp => sp.SiteId.Equals(studyId) &&
			                                   sp.ParticipantId.Equals(participantId))
                .ToListAsync();
        }

		public async Task<IEnumerable<StudyParticipant>> StudyParticipantsByParticipant(int participantId) {
			return await FindByCondition(sp => sp.ParticipantId.Equals(participantId))
				.ToListAsync();
		}

		public async Task<IEnumerable<StudyParticipant>> StudyParticipantsByStudy(int studyId, bool? abandoned) {
			return await FindByCondition(sp => sp.StudyId.Equals(studyId) && (abandoned != null ? sp.Abandoned.Equals(abandoned) : true))
				.ToListAsync();
		}

		public async Task<IEnumerable<StudyParticipant>> StudyParticipantsBySite(int siteId, bool? abandoned) {
			return await FindByCondition(sp => sp.SiteId.Equals(siteId) && (abandoned != null ? sp.Abandoned.Equals(abandoned) : true))
				.ToListAsync();
		}

		public async Task<IEnumerable<StudyParticipant>> StudyParticipantsByGroup(int groupId, bool? abandoned) {
			return await FindByCondition(sp => sp.GroupId.Equals(groupId) && (abandoned != null ? sp.Abandoned.Equals(abandoned) : true))
				.ToListAsync();
		}

        public void UpdateStudyParticipant(StudyParticipant dbStudyParticipant, StudyParticipant studyParticipant) {
            dbStudyParticipant.Map(studyParticipant);
            Update(dbStudyParticipant);
        }

    }
}
