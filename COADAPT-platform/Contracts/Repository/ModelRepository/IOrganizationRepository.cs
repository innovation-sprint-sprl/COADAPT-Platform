using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using ApiModels;

namespace Contracts.Repository.ModelRepository {

	public interface IOrganizationRepository : IRepositoryBase<Organization> {

		Task<IEnumerable<OrganizationListResponse>> GetAllOrganizationsAsync();
		Task<Organization> GetOrganizationByIdAsync(int organizationId);
		Task<Organization> GetOrganizationByShortnameAsync(string shortName);
		Task<Organization> GetOrganizationBySubAdministratorIdAsync(int subAdministratorId);
		void CreateOrganization(Organization organization);
		void UpdateOrganization(Organization dbOrganization, Organization organization);
		void DeleteOrganization(Organization organization);

	}

}
