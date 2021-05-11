using System.Threading.Tasks;
using Entities.Models;

namespace Contracts.Repository.ModelRepository {

	public interface IUserAccessTokenRepository : IRepositoryBase<UserAccessToken> {

		Task<UserAccessToken> GetRefreshToken(string refreshToken);
		Task UpsertRefreshTokenAsync(string userId, string refreshToken);

	}

}