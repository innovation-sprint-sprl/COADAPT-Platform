using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using ApiModels;

namespace Contracts.Repository.ModelRepository {

	public interface ISiteRepository : IRepositoryBase<Site> {

		Task<IEnumerable<Site>> SitesByStudy(int studyId);
		Task<IEnumerable<SiteListResponse>> SiteListByStudy(int studyId);
		Task<int> CountSitesByStudy(int studyId);
		Task<IEnumerable<SiteListResponse>> GetAllSitesAsync();
		Task<Site> GetSiteByIdAsync(int siteId);
		Task<Site> GetSiteOfStudyByShortnameAsync(string shortname, int studyId);
		void CreateSite(Site site);
		void UpdateSite(Site dbSite, Site site);
		void DeleteSite(Site site);

	}

}
