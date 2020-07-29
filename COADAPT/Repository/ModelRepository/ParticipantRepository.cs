using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.ModelRepository {
	public class ParticipantRepository : RepositoryBase<Participant>, IParticipantRepository {

		public ParticipantRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

		public void CreateParticipant(Participant participant) {
			Create(participant);
		}

		public void DeleteParticipant(Participant participant) {
			Delete(participant);
		}

		public async Task<IEnumerable<Participant>> GetAllParticipantsAsync() {
			return await FindAll().ToListAsync();
		}

		public async Task<Participant> GetParticipantByIdAsync(int participantId) {
			return await FindByCondition(p => p.Id.Equals(participantId))
				.DefaultIfEmpty(new Participant())
				.SingleAsync();
		}

		public async Task<Participant> GetParticipantByCodeAsync(string code) {
			return await FindByCondition(p => p.Code.Equals(code))
				.DefaultIfEmpty(new Participant())
				.SingleAsync();
		}

		public async Task<IEnumerable<Participant>> GetParticipantsByTherapistIdAsync(int therapistId) {
			return await FindByCondition(p => p.TherapistId.Equals(therapistId))
				.ToListAsync();
		}

		public async Task<int> CountParticipantsByTherapistIdAsync(int therapistId) {
			return await CountByCondition(p => p.TherapistId.Equals(therapistId));
		}

		public async Task<Participant> GetParticipantByUserIdAsync(string participantUserId) {
			return await FindByCondition(p => p.UserId.Equals(participantUserId))
				.DefaultIfEmpty(new Participant())
				.SingleAsync();
		}

		public void UpdateParticipant(Participant dbParticipant, Participant participant) {
			dbParticipant.Map(participant);
			Update(dbParticipant);
		}

		public void DetachParticipant(Participant participant)
		{
			Detach(participant);
		}
	}
}
