using Microsoft.AspNetCore.Identity;
using System;

namespace Entities {
	public interface IUserDetails {
		int Id { get; set; }
		DateTime CreatedOn { get; set; }
		IdentityUser User { get; set; }
	}
}
