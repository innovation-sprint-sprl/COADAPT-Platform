using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts.Repository.ModelRepository {
	public interface IAdministratorRepository {

		Task<IEnumerable<Administrator>> GetAllAdministratorsAsync();
		Task<int> CountAllAdministratorsAsync();
		Task<Administrator> GetAdministratorByIdAsync(int administratorId);
		Task<Administrator> GetAdministratorByUserIdAsync(string administratorUserId);
		void CreateAdministrator(Administrator administrator);
		void UpdateAdministrator(Administrator dbAdministrator, Administrator administrator);
		void DeleteAdministrator(Administrator administrator);

	}
}
