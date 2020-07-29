using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.ModelRepository {
	public class TherapistRepository : RepositoryBase<Therapist>, ITherapistRepository {

		public TherapistRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

		public void CreateTherapist(Therapist therapist) {
			Create(therapist);
		}

		public void DeleteTherapist(Therapist therapist) {
			Delete(therapist);
		}

		public async Task<IEnumerable<Therapist>> GetAllTherapistsAsync() {
			return await FindAll().ToListAsync();
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
