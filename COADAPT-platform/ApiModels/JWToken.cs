using System;

namespace ApiModels {

	public class JWToken {

		public string Token { get; set; }
		public string RefreshToken { get; set; }
		public DateTime? Expires { get; set; }

	}

}