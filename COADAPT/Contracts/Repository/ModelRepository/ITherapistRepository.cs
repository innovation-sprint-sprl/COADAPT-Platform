using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts.Repository.ModelRepository {

	public interface ITherapistRepository {

		Task<IEnumerable<Therapist>> GetAllTherapistsAsync();
		Task<Therapist> GetTherapistByIdAsync(int therapistId);
		Task<Therapist> GetTherapistByUserIdAsync(string therapistUserId);
		void CreateTherapist(Therapist therapist);
		void UpdateTherapist(Therapist dbTherapist, Therapist therapist);
		void DeleteTherapist(Therapist therapist);

	}
}
