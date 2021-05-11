using System.Linq;
using System.Threading.Tasks;
using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.ModelRepository {

	public class UserAccessTokenRepository : RepositoryBase<UserAccessToken>, IUserAccessTokenRepository {

		public UserAccessTokenRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

		public async Task<UserAccessToken> GetRefreshToken(string refreshToken) {
			return await FindByCondition(t => t.RefreshToken.Equals(refreshToken))
				.FirstOrDefaultAsync();
		}

		public async Task UpsertRefreshTokenAsync(string userId, string refreshToken) {
			var tokens = await FindByCondition(t => t.UserId.Equals(userId)).ToListAsync();
			DeleteRange(tokens);
			
			Create(new UserAccessToken{ UserId = userId, RefreshToken = refreshToken});
		}

	}

}