using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiModels;
using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.ModelRepository {

	public class SupervisorRepository : RepositoryBase<Supervisor>, ISupervisorRepository {

		private readonly COADAPTContext _coadaptContext;

		public SupervisorRepository(COADAPTContext coadaptContext) : base(coadaptContext) {
			_coadaptContext = coadaptContext;
		}

		public void CreateSupervisor(Supervisor supervisor) {
			Create(supervisor);
		}

		public void DeleteSupervisor(Supervisor supervisor) {
			Delete(supervisor);
		}

		public async Task<IEnumerable<SupervisorListResponse>> GetAllSupervisorsAsync() {
			return await FindAll()
				.Include(supervisor => supervisor.User)
				.Select(x => new SupervisorListResponse {
					Id = x.Id,
					UserName = x.User.UserName,
					CreatedOn = x.CreatedOn,
					Organizations = _coadaptContext.Organizations.Where(o => o.Studies.Any(s => s.OrganizationId == o.Id && s.SupervisorId == x.Id)).Select(y => y.Name).ToList(),
					Studies = _coadaptContext.Studies.Where(s => s.SupervisorId == x.Id).Select(y => y.Name).ToList(),
					Participants = _coadaptContext.StudyParticipants.Where(sp => sp.Study.SupervisorId == x.Id && sp.Abandoned == false).Count()
				})
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
