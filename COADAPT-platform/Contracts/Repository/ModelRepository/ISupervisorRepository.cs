using System.Collections.Generic;
using System.Threading.Tasks;
using ApiModels;
using Entities.Models;

namespace Contracts.Repository.ModelRepository {
	public interface ISupervisorRepository : IRepositoryBase<Supervisor> {

		Task<IEnumerable<SupervisorListResponse>> GetAllSupervisorsAsync();
		Task<Supervisor> GetSupervisorByIdAsync(int supervisorId);
		Task<Supervisor> GetSupervisorByUserIdAsync(string supervisorUserId);
		void CreateSupervisor(Supervisor supervisor);
		void UpdateSupervisor(Supervisor dbSupervisor, Supervisor supervisor);
		void DeleteSupervisor(Supervisor supervisor);

	}
}
