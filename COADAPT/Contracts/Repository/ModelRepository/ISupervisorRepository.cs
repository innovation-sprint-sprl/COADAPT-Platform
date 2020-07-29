using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts.Repository.ModelRepository {
	public interface ISupervisorRepository {

		Task<IEnumerable<Supervisor>> GetAllSupervisorsAsync();
		Task<Supervisor> GetSupervisorByIdAsync(int supervisorId);
		Task<Supervisor> GetSupervisorByUserIdAsync(string supervisorUserId);
		void CreateSupervisor(Supervisor supervisor);
		void UpdateSupervisor(Supervisor dbSupervisor, Supervisor supervisor);
		void DeleteSupervisor(Supervisor supervisor);

	}
}
