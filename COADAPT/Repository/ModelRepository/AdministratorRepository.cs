using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.ModelRepository {
	public class AdministratorRepository : RepositoryBase<Administrator>, IAdministratorRepository {
		public AdministratorRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

		public void CreateAdministrator(Administrator administrator) {
			Create(administrator);
		}

		public void DeleteAdministrator(Administrator administrator) {
			Delete(administrator);
		}

		public async Task<Administrator> GetAdministratorByIdAsync(int administratorId) {
			return await FindByCondition(admin => admin.Id.Equals(administratorId))
				//.Include(admin => admin.User)
				.DefaultIfEmpty(new Administrator())
				.SingleAsync();
		}

		public async Task<Administrator> GetAdministratorByUserIdAsync(string administratorUserId) {
			return await FindByCondition(admin => admin.UserId.Equals(administratorUserId))
				//.Include(admin => admin.User)
				.DefaultIfEmpty(new Administrator())
				.SingleAsync();
		}

		public async Task<IEnumerable<Administrator>> GetAllAdministratorsAsync() {
			return await FindAll()
				//.Include(admin => admin.User)
				.ToListAsync();
		}

		public async Task<int> CountAllAdministratorsAsync() {
			return await CountAll();
		}

		public void UpdateAdministrator(Administrator dbAdministrator, Administrator administrator) {
			dbAdministrator.Map(administrator);
			Update(dbAdministrator);
		}
	}
}
