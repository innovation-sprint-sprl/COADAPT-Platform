using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts.Repository.ModelRepository {
	public interface IParticipantRepository {

		Task<IEnumerable<Participant>> GetAllParticipantsAsync();
		Task<Participant> GetParticipantByIdAsync(int participantId);
		Task<Participant> GetParticipantByCodeAsync(string code);
		Task<IEnumerable<Participant>> GetParticipantsByTherapistIdAsync(int therapistId);
		Task<int> CountParticipantsByTherapistIdAsync(int therapistId);
		Task<Participant> GetParticipantByUserIdAsync(string ParticipantUserId);
		void CreateParticipant(Participant participant);
		void UpdateParticipant(Participant dbParticipant, Participant participant);
		void DeleteParticipant(Participant participant);
		void DetachParticipant(Participant participant);
	}
}
