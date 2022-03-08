using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repository.ModelRepository {
    public interface IOuraActivityRepository : IRepositoryBase<OuraActivity> {
        Task<IEnumerable<OuraActivity>> GetOuraActivitiesAsync();
        Task<OuraActivity> GetOuraActivityByIdAsync(int id);
        bool Exists(int participantId, DateTime summaryDate);
        Task<IEnumerable<OuraActivity>> GetOuraActivitiesByParticipantIdAsync(int participantId);
        Task<IEnumerable<OuraActivity>> GetOuraActivitiesByParticipantIdAndDateRangeAsync(
            int participantId, DateTime fromDate, DateTime toDate);
        Task<OuraActivity> GetOuraActivityByParticipantIdAndDateAsync(int participantId, DateTime date);
        Task<IEnumerable<OuraActivity>> GetOuraActivitiesByDateRangeAsync(DateTime fromDate, DateTime toDate);
        void CreateOuraActivity(OuraActivity ouraActivity);
        void DeleteOuraActivity(OuraActivity ouraActivity);
        void UpdateOuraActivity(OuraActivity dbOuraActivity, OuraActivity ouraActivity);
    }
}
