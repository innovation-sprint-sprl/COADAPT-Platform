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

	public class StudyRepository : RepositoryBase<Study>, IStudyRepository {

		private readonly COADAPTContext _coadaptContext;

		public StudyRepository(COADAPTContext coadaptContext) : base(coadaptContext) { 
			_coadaptContext = coadaptContext;
		}

		public async Task<IEnumerable<StudyListResponse>> StudiesBySupervisor(int supervisorId) {
			return await FindByCondition(s => s.SupervisorId.Equals(supervisorId))
				.Include(s => s.Supervisor)
				.ThenInclude(s => s.User)
				.Include(s => s.Organization)
				.Select(s => new StudyListResponse
				{
					Id = s.Id,
					Name = s.Name,
					Shortname = s.Shortname,
					Organization = s.Organization.Name,
					Supervisor = s.Supervisor.User.UserName,
					Sites = _coadaptContext.Sites.Where(st => st.StudyId == s.Id).Count(),
					Groups = _coadaptContext.Groups.Where(g => g.StudyId == s.Id).Count(),
					Participants = _coadaptContext.StudyParticipants.Where(sp => sp.StudyId == s.Id && sp.Abandoned == false).Count()
				})
				.ToListAsync();
		}

		public async Task<IEnumerable<StudyListResponse>> StudiesByOrganization(int organizationId) {
			return await FindByCondition(s => s.Organization.Id.Equals(organizationId))
				.Include(x => x.Supervisor)
				.ThenInclude(s => s.User)
				.Include(x => x.Organization)
				.Select(s => new StudyListResponse
				{
					Id = s.Id,
					Name = s.Name,
					Shortname = s.Shortname,
					Organization = s.Organization.Name,
					Supervisor = s.Supervisor.User.UserName,
					Sites = _coadaptContext.Sites.Where(st => st.StudyId == s.Id).Count(),
					Groups = _coadaptContext.Groups.Where(g => g.StudyId == s.Id).Count(),
					Participants = _coadaptContext.StudyParticipants.Where(sp => sp.StudyId == s.Id).Count()
				})
				.ToListAsync();
		}

		public async Task<IEnumerable<StudyListResponse>> GetAllStudiesAsync() {
			return await FindAll()
				.Include(s => s.Supervisor)
				.ThenInclude(s => s.User)
				.Include(s => s.Organization)
				.Select(s => new StudyListResponse
				{
					Id = s.Id,
					Name = s.Name,
					Shortname = s.Shortname,
					Organization = s.Organization.Name,
					Supervisor = s.Supervisor.User.UserName,
					Sites = _coadaptContext.Sites.Where(st => st.StudyId == s.Id).Count(),
					Groups = _coadaptContext.Groups.Where(g => g.StudyId == s.Id).Count(),
					Participants = _coadaptContext.StudyParticipants.Where(sp => sp.StudyId == s.Id).Count()
				})
				.ToListAsync();
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
				.Include(x => x.Supervisor)
				.ThenInclude(s => s.User)
				.Include(x => x.Organization)
				.ToListAsync();
		}

		public async Task<IEnumerable<Study>> GetStudiesByOrganizationIdAsync(int organizationId) {
			return await FindByCondition(s => s.OrganizationId.Equals(organizationId))
				.Include(s => s.Supervisor)
				.ThenInclude(s => s.User)
				.Include(s => s.Organization)
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
