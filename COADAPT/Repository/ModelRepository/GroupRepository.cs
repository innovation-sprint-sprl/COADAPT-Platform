using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.ModelRepository {
    public class GroupRepository : RepositoryBase<Group>, IGroupRepository {
        public GroupRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

        public async Task<IEnumerable<Group>> GroupsByStudy(int studyId) {
            return await FindByCondition(g => g.StudyId.Equals(studyId))
                .ToListAsync();
        }

        public async Task<int> CountGroupsByStudy(int studyId) {
            return await CountByCondition(g => g.StudyId.Equals(studyId));
        }

        public async Task<IEnumerable<Group>> GetAllGroupsAsync() {
            return await FindAll().ToListAsync();
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