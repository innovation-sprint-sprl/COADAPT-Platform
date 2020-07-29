using Entities.Models;

namespace Entities.Extensions {
	public static class SupervisorExtensions {
		public static void Map(this Supervisor dbSupervisor, Supervisor supervisor) {
			dbSupervisor.UserId = supervisor.UserId;
		}
	}
}
