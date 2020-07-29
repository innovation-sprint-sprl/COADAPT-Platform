using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.ModelRepository {

	public class SupervisorRepository : RepositoryBase<Supervisor>, ISupervisorRepository {

		public SupervisorRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

		public void CreateSupervisor(Supervisor supervisor) {
			Create(supervisor);
		}

		public void DeleteSupervisor(Supervisor supervisor) {
			Delete(supervisor);
		}

		public async Task<IEnumerable<Supervisor>> GetAllSupervisorsAsync() {
			return await FindAll()
				//.Include(supervisor => supervisor.User)
				.ToListAsync();
		}

		public async Task<Supervisor> GetSupervisorByIdAsync(int supervisorId) {
			return await FindByCondition(super => super.Id.Equals(supervisorId))
				//.Include(super => super.User)
				.DefaultIfEmpty(new Supervisor())
				.SingleAsync();
		}

		public async Task<Supervisor> GetSupervisorByUserIdAsync(string supervisorUserId) {
			return await FindByCondition(super => super.UserId.Equals(supervisorUserId))
				//.Include(super => super.User)
				.DefaultIfEmpty(new Supervisor())
				.SingleAsync();
		}

		public void UpdateSupervisor(Supervisor dbSupervisor, Supervisor supervisor) {
			dbSupervisor.Map(supervisor);
			Update(dbSupervisor);
		}
	}
}
