using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts.Repository.ModelRepository {

	public interface ISiteRepository {

		Task<IEnumerable<Site>> SitesByStudy(int studyId);
		Task<int> CountSitesByStudy(int studyId);
		Task<IEnumerable<Site>> GetAllSitesAsync();
		Task<Site> GetSiteByIdAsync(int siteId);
		Task<Site> GetSiteOfStudyByShortnameAsync(string shortname, int studyId);
		void CreateSite(Site site);
		void UpdateSite(Site dbSite, Site site);
		void DeleteSite(Site site);

	}

}
