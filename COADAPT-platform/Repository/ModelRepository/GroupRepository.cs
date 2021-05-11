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

	public class GroupRepository : RepositoryBase<Group>, IGroupRepository {

		private readonly COADAPTContext _coadaptContext;

		public GroupRepository(COADAPTContext coadaptContext) : base(coadaptContext) {
			_coadaptContext = coadaptContext;
		}

		public async Task<IEnumerable<Group>> GroupsByStudy(int studyId) {
			return await FindByCondition(g => g.StudyId.Equals(studyId))
				.Include(g => g.Study)
				.ToListAsync();
		}

		public async Task<IEnumerable<GroupListResponse>> GroupListByStudy(int studyId) {
			return await FindByCondition(g => g.StudyId.Equals(studyId))
				.Include(g => g.Study)
				.Select(g => new GroupListResponse {
					Id = g.Id,
					Name = g.Name,
					Shortname = g.Shortname,
					Organization = g.Study.Organization.Name,
					StudyId = g.StudyId,
					Study = g.Study.Name,
					StudyShortname = g.Study.Shortname,
					Participants = _coadaptContext.StudyParticipants.Where(sp => sp.GroupId == g.Id && sp.Abandoned == false).Count()
				})
				.ToListAsync();
		}

		public async Task<int> CountGroupsByStudy(int studyId) {
			return await CountByCondition(g => g.StudyId.Equals(studyId));
		}

		public async Task<IEnumerable<GroupListResponse>> GetAllGroupsAsync() {
			return await FindAll()
				.Include(g => g.Study)
				.Select(g => new GroupListResponse {
					Id = g.Id,
					Name = g.Name,
					Shortname = g.Shortname,
					Organization = g.Study.Organization.Name,
					StudyId = g.StudyId,
					Study = g.Study.Name,
					StudyShortname = g.Study.Shortname,
					Participants = _coadaptContext.StudyParticipants.Where(sp => sp.GroupId == g.Id && sp.Abandoned == false).Count()
				})
				.ToListAsync();
		}

		public async Task<Group> GetGroupByIdAsync(int groupId) {
			return await FindByCondition(g => g.Id.Equals(groupId))
				.DefaultIfEmpty(new Group())
				.SingleAsync();
		}

		public async Task<Group> GetGroupOfStudyByShortnameAsync(string shortName, int studyId) {
			return await FindByCondition(g => g.Shortname.Equals(shortName) && g.StudyId.Equals(studyId))
				.DefaultIfEmpty(new Group())
				.SingleAsync();
		}

		public void CreateGroup(Group group) {
			Create(group);
		}

		public void UpdateGroup(Group dbGroup, Group group) {
			dbGroup.Map(group);
			Update(dbGroup);
		}

		public void DeleteGroup(Group group) {
			Delete(group);
		}

	}

}
