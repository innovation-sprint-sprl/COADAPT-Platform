using Entities.Models;

namespace Entities.Extensions {
	public static class AdministratorExtensions {

		public static void Map(this Administrator dbAdministrator, Administrator administrator) {
			dbAdministrator.UserId = administrator.UserId;
			dbAdministrator.CreatedOn = administrator.CreatedOn;
		}
	}
}
