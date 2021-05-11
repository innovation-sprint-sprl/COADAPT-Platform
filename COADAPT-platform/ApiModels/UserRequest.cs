using System.ComponentModel.DataAnnotations;

namespace ApiModels {

	public class UserRequest {

		[Required]
		public string UserName { get; set; }

		[Required]
		public string Password { get; set; }

	}

}
