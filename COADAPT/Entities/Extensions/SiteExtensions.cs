using Entities.Models;

namespace Entities.Extensions {

	public static class SiteExtensions {

		public static void Map(this Site dbSite, Site site) {
			dbSite.Name = site.Name;
			dbSite.Shortname = site.Shortname;
			dbSite.StudyId = site.StudyId;
		}

	}

}
