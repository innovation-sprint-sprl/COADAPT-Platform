using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using ApiModels;

namespace Contracts.Repository.ModelRepository {
	public interface IParticipantRepository : IRepositoryBase<Participant> {

		Task<IEnumerable<Participant>> GetAllParticipantsAsync();
		Task<IEnumerable<ParticipantListResponse>> GetParticipantListAsync();
		Task<Participant> GetParticipantByIdAsync(int participantId);
		Task<ParticipantListResponse> GetParticipantListItemByIdAsync(int participantId);
		Task<Participant> GetParticipantByCodeAsync(string code);
		Task<IEnumerable<Participant>> GetParticipantsByTherapistIdAsync(int therapistId);
		Task<int> CountParticipantsByTherapistIdAsync(int therapistId);
		Task<Participant> GetParticipantByUserIdAsync(string participantUserId);
		void CreateParticipant(Participant participant);
		void UpdateParticipant(Participant dbParticipant, Participant participant);
		void DeleteParticipant(Participant participant);
		void DetachParticipant(Participant participant);
	}
}
