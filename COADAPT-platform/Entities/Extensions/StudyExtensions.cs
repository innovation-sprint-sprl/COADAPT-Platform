using Entities.Models;

namespace Entities.Extensions {

	public static class StudyExtensions {

		public static void Map(this Study dbStudy, Study study) {
			dbStudy.Name = study.Name;
			dbStudy.Shortname = study.Shortname;
			dbStudy.OrganizationId = study.OrganizationId;
			dbStudy.SupervisorId = study.SupervisorId;
		}

	}

}
