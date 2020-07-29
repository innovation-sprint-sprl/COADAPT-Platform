using System.Collections.Generic;

namespace ApiModels {
	public class LoginResponse {
		public string Id { get; set; }
		public string UserName { get; set; }
		public IEnumerable<string> Roles { get; set; }
		public string Token { get; set; }
	}
}
