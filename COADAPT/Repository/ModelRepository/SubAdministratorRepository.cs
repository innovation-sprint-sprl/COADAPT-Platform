using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.ModelRepository {
	public class SubAdministratorRepository : RepositoryBase<SubAdministrator>, ISubAdministratorRepository {

		public SubAdministratorRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

		public void CreateSubAdministrator(SubAdministrator subAdministrator) {
			Create(subAdministrator);
		}

		public void DeleteSubAdministrator(SubAdministrator subAdministrator) {
			Delete(subAdministrator);
		}

		public async Task<IEnumerable<SubAdministrator>> GetAllSubAdministratorsAsync() {
			return await FindAll()
				//.Include(subadmin => subadmin.User)
				.ToListAsync();
		}

		public async Task<SubAdministrator> GetSubAdministratorByIdAsync(int subAdministratorId) {
			return await FindByCondition(subadmin => subadmin.Id.Equals(subAdministratorId))
				//.Include(subadmin => subadmin.User)
				.DefaultIfEmpty(new SubAdministrator())
				.SingleAsync();
		}

		public async Task<SubAdministrator> GetSubAdministratorByUserIdAsync(string userId) {
			return await FindByCondition(subadmin => subadmin.UserId.Equals(userId))
				//.Include(subadmin => subadmin.User)
				.DefaultIfEmpty(new SubAdministrator())
				.SingleAsync();
		}

		public void UpdateSubAdministrator(SubAdministrator dbSubAdministrator, SubAdministrator subAdministrator) {
			dbSubAdministrator.Map(subAdministrator);
			Update(dbSubAdministrator);
		}
	}
}
