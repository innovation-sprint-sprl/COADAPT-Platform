using System.ComponentModel.DataAnnotations;

namespace ApiModels {

	public class RefreshRequest {

		[Required]
		public string RefreshToken { get; set; }

	}

}
