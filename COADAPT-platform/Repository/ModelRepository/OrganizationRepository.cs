using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiModels;

namespace Repository.ModelRepository {
	public class OrganizationRepository : RepositoryBase<Organization>, IOrganizationRepository {

		private readonly COADAPTContext _coadaptContext;

		public OrganizationRepository(COADAPTContext coadaptContext) : base(coadaptContext) {
			_coadaptContext = coadaptContext;
		}

		public void CreateOrganization(Organization organization) {
			Create(organization);
		}

		public void DeleteOrganization(Organization organization) {
			Delete(organization);
		}

		public async Task<IEnumerable<OrganizationListResponse>> GetAllOrganizationsAsync() {
			return await FindAll()
				.Select(o => new OrganizationListResponse {
					Id = o.Id,
					Name = o.Name,
					Shortname = o.Shortname,
					SubAdministrator = o.SubAdministrator.User.UserName,
					Studies = _coadaptContext.Studies.Where(s => s.OrganizationId == o.Id).Count(),
					Participants = _coadaptContext.Participants.Where(p => p.StudyParticipants.Any(sp => sp.Study.OrganizationId == o.Id && sp.ParticipantId == p.Id)).Count()
				})
				.ToListAsync();
		}

		public async Task<Organization> GetOrganizationByIdAsync(int organizationId) {
			return await FindByCondition(org => org.Id.Equals(organizationId))
				.DefaultIfEmpty(new Organization())
				.SingleAsync();
		}

		public async Task<Organization> GetOrganizationByShortnameAsync(string shortName) {
			return await FindByCondition(org => org.Shortname.Equals(shortName))
				.DefaultIfEmpty(new Organization())
				.SingleAsync();
		}

		public async Task<Organization> GetOrganizationBySubAdministratorIdAsync(int subAdministratorId) {
			return await FindByCondition(org => org.SubAdministratorId.Equals(subAdministratorId))
				.Include(x => x.SubAdministrator)
				.ThenInclude(s => s.User)
				.DefaultIfEmpty(new Organization())
				.SingleAsync();
		}

		public void UpdateOrganization(Organization dbOrganization, Organization organization) {
			dbOrganization.Map(organization);
			Update(dbOrganization);
		}
	}
}
