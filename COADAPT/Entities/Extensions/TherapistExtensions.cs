using Entities.Models;

namespace Entities.Extensions {
	public static class TherapistExtensions {
		public static void Map(this Therapist dbTherapist, Therapist therapist) {
			dbTherapist.UserId = therapist.UserId;
		}
	}
}
