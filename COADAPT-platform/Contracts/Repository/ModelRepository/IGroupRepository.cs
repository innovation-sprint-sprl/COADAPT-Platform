using System.Collections.Generic;
using System.Threading.Tasks;
using ApiModels;
using Entities.Models;

namespace Contracts.Repository.ModelRepository {
    public interface IGroupRepository : IRepositoryBase<Group> {
        Task<IEnumerable<Group>> GroupsByStudy(int studyId);
        Task<IEnumerable<GroupListResponse>> GroupListByStudy(int studyId);
        Task<int> CountGroupsByStudy(int studyId);
        Task<IEnumerable<GroupListResponse>> GetAllGroupsAsync();
        Task<Group> GetGroupByIdAsync(int groupId);
        Task<Group> GetGroupOfStudyByShortnameAsync(string shortname, int studyId);
        void CreateGroup(Group group);
        void UpdateGroup(Group dbGroup, Group group);
        void DeleteGroup(Group group);
    }
}