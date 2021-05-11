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
	public class TherapistRepository : RepositoryBase<Therapist>, ITherapistRepository {

		private readonly COADAPTContext _coadaptContext;

		public TherapistRepository(COADAPTContext coadaptContext) : base(coadaptContext) {
			_coadaptContext = coadaptContext;
		}

		public void CreateTherapist(Therapist therapist) {
			Create(therapist);
		}

		public void DeleteTherapist(Therapist therapist) {
			Delete(therapist);
		}

		public async Task<IEnumerable<TherapistResponse>> GetAllTherapistsAsync() {
			return await FindAll()
				.Include(x => x.User)
				.Select(x => new TherapistResponse {
					Id = x.Id,
					CreatedOn = x.CreatedOn,
					UserName = x.User.UserName,
					Participants = _coadaptContext.Participants.Where(p => p.TherapistId == x.Id).Count()
				})
				.ToListAsync();
		}

		public async Task<Therapist> GetTherapistByIdAsync(int therapistId) {
			return await FindByCondition(therapist => therapist.Id.Equals(therapistId))
				.DefaultIfEmpty(new Therapist())
				.SingleAsync();
		}

		public async Task<Therapist> GetTherapistByUserIdAsync(string therapistUserId) {
			return await FindByCondition(therapist => therapist.UserId.Equals(therapistUserId))
				.DefaultIfEmpty(new Therapist())
				.SingleAsync();
		}

		public void UpdateTherapist(Therapist dbTherapist, Therapist therapist) {
			dbTherapist.Map(therapist);
			Update(dbTherapist);
		}
	}
}
