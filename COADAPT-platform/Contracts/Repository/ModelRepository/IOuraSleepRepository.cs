using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repository.ModelRepository {
    public interface IOuraSleepRepository : IRepositoryBase<OuraSleep> {
        Task<IEnumerable<OuraSleep>> GetOuraSleepsAsync();
        Task<OuraSleep> GetOuraSleepByIdAsync(int id);
        Task<IEnumerable<OuraSleep>> GetOuraSleepsByParticipantIdAsync(int participantId);
        Task<IEnumerable<OuraSleep>> GetOuraSleepsByParticipantIdAndDateRangeAsync(
            int participantId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<OuraSleep>> GetOuraSleepsByDateRangeAsync(
            DateTime fromDate, DateTime toDate);
        Task<OuraSleep> GetOuraSleepByParticipantIdAndDateAsync(int participantId, DateTime date);
        void CreateOuraSleep(OuraSleep ouraSleep);
        void DeleteOuraSleep(OuraSleep ouraSleep);
        void UpdateOuraSleep(OuraSleep dbOuraSleep, OuraSleep ouraSleep);
    }
}
