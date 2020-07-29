using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts.Repository.ModelRepository {

	public interface IOrganizationRepository {

		Task<IEnumerable<Organization>> GetAllOrganizationsAsync();
		Task<Organization> GetOrganizationByIdAsync(int organizationId);
		Task<Organization> GetOrganizationByShortnameAsync(string shortName);
		Task<Organization> GetOrganizationBySubAdministratorIdAsync(int subAdministratorId);
		void CreateOrganization(Organization organization);
		void UpdateOrganization(Organization dbOrganization, Organization organization);
		void DeleteOrganization(Organization organization);

	}

}
