using Entities.Models;

namespace Entities.Extensions {
	public static class SubAdministratorExtensions {
		public static void Map(this SubAdministrator dbSubAdministrator, SubAdministrator subAdministrator) {
			dbSubAdministrator.UserId = subAdministrator.UserId;
		}
	}
}
