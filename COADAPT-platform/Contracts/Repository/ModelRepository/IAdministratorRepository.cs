using System.Collections.Generic;
using System.Threading.Tasks;
using ApiModels;
using Entities.Models;

namespace Contracts.Repository.ModelRepository {
	public interface IAdministratorRepository : IRepositoryBase<Administrator> {

		Task<IEnumerable<AdministratorResponse>> GetAllAdministratorsAsync();
		Task<int> CountAllAdministratorsAsync();
		Task<Administrator> GetAdministratorByIdAsync(int administratorId);
		Task<Administrator> GetAdministratorByUserIdAsync(string administratorUserId);
		void CreateAdministrator(Administrator administrator);
		void UpdateAdministrator(Administrator dbAdministrator, Administrator administrator);
		void DeleteAdministrator(Administrator administrator);

	}
}
