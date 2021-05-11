using Entities.Models;

namespace Entities.Extensions {
	public static class TherapistExtensions {
		public static void Map(this Therapist dbTherapist, Therapist therapist) {
			dbTherapist.UserId = therapist.UserId;
			dbTherapist.CreatedOn = therapist.CreatedOn;
			dbTherapist.Birthday = therapist.Birthday;
			dbTherapist.Name = therapist.Name;
			dbTherapist.Gender = therapist.Gender;
		}
	}
}
