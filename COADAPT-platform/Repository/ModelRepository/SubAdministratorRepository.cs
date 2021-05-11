using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiModels;
using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.ModelRepository {
	public class SubAdministratorRepository : RepositoryBase<SubAdministrator>, ISubAdministratorRepository {

		private readonly COADAPTContext _coadaptContext;

		public SubAdministratorRepository(COADAPTContext coadaptContext) : base(coadaptContext) {
			_coadaptContext = coadaptContext;
		}

		public void CreateSubAdministrator(SubAdministrator subAdministrator) {
			Create(subAdministrator);
		}

		public void DeleteSubAdministrator(SubAdministrator subAdministrator) {
			Delete(subAdministrator);
		}

		public async Task<IEnumerable<SubAdministratorListResponse>> GetAllSubAdministratorsAsync() {
			return await FindAll()
				.Include(subadmin => subadmin.User)
				.Select(s => new SubAdministratorListResponse {
					Id = s.Id,
					UserName = s.User.UserName,
					CreatedOn = s.CreatedOn,
					Organization = _coadaptContext.Organizations.Where(o => o.SubAdministratorId == s.Id).Select(y => y.Name).FirstOrDefault(),
					Participants = _coadaptContext.Participants.Where(p => p.StudyParticipants.Any(sp => sp.Study.Organization.SubAdministratorId == s.Id && sp.ParticipantId == p.Id)).Count()
				})
				.ToListAsync();
		}

		public async Task<SubAdministrator> GetSubAdministratorByIdAsync(int subAdministratorId) {
			return await FindByCondition(subadmin => subadmin.Id.Equals(subAdministratorId))
				//.Include(subadmin => subadmin.User)
				.DefaultIfEmpty(new SubAdministrator())
				.SingleAsync();
		}

		public async Task<SubAdministrator> GetSubAdministratorByUserIdAsync(string userId) {
			return await FindByCondition(subadmin => subadmin.UserId.Equals(userId))
				//.Include(subadmin => subadmin.User)
				.DefaultIfEmpty(new SubAdministrator())
				.SingleAsync();
		}

		public void UpdateSubAdministrator(SubAdministrator dbSubAdministrator, SubAdministrator subAdministrator) {
			dbSubAdministrator.Map(subAdministrator);
			Update(dbSubAdministrator);
		}
	}
}
