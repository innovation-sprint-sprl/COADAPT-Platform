using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using ApiModels;

namespace Repository.ModelRepository {

	public class SiteRepository : RepositoryBase<Site>, ISiteRepository {

		private readonly COADAPTContext _coadaptContext;

		public SiteRepository(COADAPTContext coadaptContext) : base(coadaptContext) {
			_coadaptContext = coadaptContext;
		}

		public async Task<IEnumerable<Site>> SitesByStudy(int studyId) {
			return await FindByCondition(s => s.StudyId.Equals(studyId))
				.Include(s => s.Study)
				.ToListAsync();
		}

		public async Task<IEnumerable<SiteListResponse>> SiteListByStudy(int studyId) {
			return await FindByCondition(g => g.StudyId.Equals(studyId))
				.Include(s => s.Study)
				.Select(s => new SiteListResponse
				{
					Id = s.Id,
					Name = s.Name,
					Shortname = s.Shortname,
					Organization = s.Study.Organization.Name,
					StudyId = s.StudyId,
					Study = s.Study.Name,
					StudyShortname = s.Study.Shortname,
					Participants = _coadaptContext.StudyParticipants.Where(sp => sp.SiteId == s.Id && sp.Abandoned == false).Count()
				})
				.ToListAsync();
		}

		public async Task<int> CountSitesByStudy(int studyId) {
			return await CountByCondition(s => s.StudyId.Equals(studyId));
		}

		public async Task<IEnumerable<SiteListResponse>> GetAllSitesAsync() {
			return await FindAll()
				.Include(s => s.Study)
				.Select(s => new SiteListResponse {
					Id = s.Id,
					Name = s.Name,
					Shortname = s.Shortname,
					Organization = s.Study.Organization.Name,
					StudyId = s.StudyId,
					Study = s.Study.Name,
					StudyShortname = s.Study.Shortname,
					Participants = _coadaptContext.StudyParticipants.Where(sp => sp.SiteId == s.Id && sp.Abandoned == false).Count()
				})
				.ToListAsync();
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
