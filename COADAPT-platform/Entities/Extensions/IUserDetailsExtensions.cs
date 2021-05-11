using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Extensions {
	public static class IUserDetailsExtensions {

		public static bool IsObjectNull(this IUserDetails model) {
			return model == null;
		}

		public static bool IsEmptyObject(this IUserDetails model) {
			return model.Id == 0;
		}

	}
}
