using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.ModelRepository {

	public class SiteRepository : RepositoryBase<Site>, ISiteRepository {

		public SiteRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

		public async Task<IEnumerable<Site>> SitesByStudy(int studyId) {
			return await FindByCondition(s => s.StudyId.Equals(studyId))
				.ToListAsync();
		}

		public async Task<int> CountSitesByStudy(int studyId) {
			return await CountByCondition(s => s.StudyId.Equals(studyId));
		}

		public async Task<IEnumerable<Site>> GetAllSitesAsync() {
			return await FindAll().ToListAsync();
		}

		public async Task<Site> GetSiteByIdAsync(int siteId) {
			return await FindByCondition(s => s.Id.Equals(siteId))
				.DefaultIfEmpty(new Site())
				.SingleAsync();
		}

		public async Task<Site> GetSiteOfStudyByShortnameAsync(string shortName, int studyId) {
			return await FindByCondition(s => s.Shortname.Equals(shortName) && s.StudyId.Equals(studyId))
				.DefaultIfEmpty(new Site())
				.SingleAsync();
		}

		public void CreateSite(Site site) {
			Create(site);
		}

		public void UpdateSite(Site dbSite, Site site) {
			dbSite.Map(site);
			Update(dbSite);
		}

		public void DeleteSite(Site site) {
			Delete(site);
		}

	}

}
