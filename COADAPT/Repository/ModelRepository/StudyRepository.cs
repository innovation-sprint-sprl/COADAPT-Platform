using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.ModelRepository {

	public class StudyRepository : RepositoryBase<Study>, IStudyRepository {

		public StudyRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

		public async Task<IEnumerable<Study>> StudiesBySupervisor(int supervisorId) {
			return await FindByCondition(s => s.SupervisorId.Equals(supervisorId))
				.ToListAsync();
		}

		public async Task<IEnumerable<Study>> StudiesByOrganization(int organizationId) {
			return await FindByCondition(s => s.Organization.Id.Equals(organizationId))
				.ToListAsync();
		}

		public async Task<IEnumerable<Study>> GetAllStudiesAsync() {
			return await FindAll().ToListAsync();
		}

		public async Task<Study> GetStudyByIdAsync(int studyId) {
			return await FindByCondition(s => s.Id.Equals(studyId))
				.DefaultIfEmpty(new Study())
				.SingleAsync();
		}

		public async Task<Study> GetStudyOfOrganizationByShortnameAsync(string shortname, int organizationId) {
			return await FindByCondition(s => s.Shortname.Equals(shortname) && s.OrganizationId.Equals(organizationId))
				.DefaultIfEmpty(new Study())
				.SingleAsync();
		}

		public async Task<IEnumerable<Study>> GetStudiesBySupervisorIdAsync(int supervisorId) {
			return await FindByCondition(s => s.SupervisorId.Equals(supervisorId))
				.ToListAsync();
		}

		public async Task<IEnumerable<Study>> GetStudiesByOrganizationIdAsync(int organizationId) {
			return await FindByCondition(s => s.OrganizationId.Equals(organizationId))
				.ToListAsync();
		}

		public async Task<int> CountStudiesBySupervisorIdAsync(int supervisorId) {
			return await CountByCondition(s => s.SupervisorId.Equals(supervisorId));
		}

		public async Task<int> CountStudiesByOrganizationIdAsync(int organizationId) {
			return await CountByCondition(s => s.OrganizationId.Equals(organizationId));
		}

		public void CreateStudy(Study study) {
			Create(study);
		}

		public void UpdateStudy(Study dbStudy, Study study) {
			dbStudy.Map(study);
			Update(dbStudy);
		}

		public void DeleteStudy(Study study) {
			Delete(study);
		}
	}

}
