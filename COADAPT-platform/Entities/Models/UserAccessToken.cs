using Microsoft.AspNetCore.Identity;

namespace Entities.Models {

	public class UserAccessToken {

		public string UserId { get; set; }
		public string RefreshToken { get; set; }

		public virtual IdentityUser User { get; set; }

	}

}