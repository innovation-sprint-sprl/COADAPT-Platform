using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.ModelRepository {
	public class OrganizationRepository : RepositoryBase<Organization>, IOrganizationRepository {
		public OrganizationRepository(COADAPTContext coadaptContext) : base(coadaptContext) {
		}

		public void CreateOrganization(Organization organization) {
			Create(organization);
		}

		public void DeleteOrganization(Organization organization) {
			Delete(organization);
		}

		public async Task<IEnumerable<Organization>> GetAllOrganizationsAsync() {
			return await FindAll().ToListAsync();
		}

		public async Task<Organization> GetOrganizationByIdAsync(int organizationId) {
			return await FindByCondition(org => org.Id.Equals(organizationId))
				.DefaultIfEmpty(new Organization())
				.SingleAsync();
		}

		public async Task<Organization> GetOrganizationByShortnameAsync(string shortName) {
			return await FindByCondition(org => org.Shortname.Equals(shortName))
				.DefaultIfEmpty(new Organization())
				.SingleAsync();
		}

		public async Task<Organization> GetOrganizationBySubAdministratorIdAsync(int subAdministratorId) {
			return await FindByCondition(org => org.SubAdministratorId.Equals(subAdministratorId))
				.DefaultIfEmpty(new Organization())
				.SingleAsync();
		}

		public void UpdateOrganization(Organization dbOrganization, Organization organization) {
			dbOrganization.Map(organization);
			Update(dbOrganization);
		}
	}
}
