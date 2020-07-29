using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts.Repository.ModelRepository {
	public interface ISubAdministratorRepository {

		Task<IEnumerable<SubAdministrator>> GetAllSubAdministratorsAsync();
		Task<SubAdministrator> GetSubAdministratorByIdAsync(int subAdministratorId);
		Task<SubAdministrator> GetSubAdministratorByUserIdAsync(string userId);
		void CreateSubAdministrator(SubAdministrator subAdministrator);
		void UpdateSubAdministrator(SubAdministrator dbSubAdministrator, SubAdministrator subAdministrator);
		void DeleteSubAdministrator(SubAdministrator subAdministrator);

	}
}
