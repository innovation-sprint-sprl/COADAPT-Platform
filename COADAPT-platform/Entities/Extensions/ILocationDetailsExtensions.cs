namespace Entities.Extensions {

	public static class ILocationDetailsExtensions {

		public static bool IsObjectNull(this ILocationDetails model) {
			return model == null;
		}

		public static bool IsEmptyObject(this ILocationDetails model) {
			return model.Id == 0;
		}

		public static void CreateShortname(this ILocationDetails model) {
			var shortname = model.Name;
		}

	}

}
