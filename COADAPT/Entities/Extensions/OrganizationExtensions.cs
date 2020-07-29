using Entities.Models;

namespace Entities.Extensions {
	public static class OrganizationExtensions {

		public static void Map(this Organization dbOrganization, Organization organization) {
			dbOrganization.Name = organization.Name;
			dbOrganization.Shortname = organization.Shortname;
			dbOrganization.SubAdministratorId = organization.SubAdministratorId;
		}

	}
}
