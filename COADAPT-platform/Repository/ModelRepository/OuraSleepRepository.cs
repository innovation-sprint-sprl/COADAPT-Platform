using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.ModelRepository {
    public class OuraSleepRepository : RepositoryBase<OuraSleep>, IOuraSleepRepository {
        public OuraSleepRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

        public async Task<IEnumerable<OuraSleep>> GetOuraSleepsAsync() {
            return await FindAll().ToListAsync();
        }

        public async Task<OuraSleep> GetOuraSleepByIdAsync(int id) {
            return await FindByCondition(p => p.Id == id)
                .DefaultIfEmpty(new OuraSleep())
                .SingleAsync();
        }

        public bool Exists(int participantId, DateTime summaryDate) {
            return FindByCondition(x => x.ParticipantId == participantId && x.SummaryDate == summaryDate).Any();
        }

        public async Task<IEnumerable<OuraSleep>> GetOuraSleepsByParticipantIdAsync(int participantId) {
            return await FindByCondition(p => p.ParticipantId.Equals(participantId))
                .ToListAsync();
        }

        public async Task<IEnumerable<OuraSleep>> GetOuraSleepsByParticipantIdAndDateRangeAsync(
            int participantId, DateTime fromDate, DateTime toDate) {
            return await FindByCondition(p => p.SummaryDate.CompareTo(fromDate) >= 0 &&
                                              p.SummaryDate.CompareTo(toDate) < 0 &&
                                              p.ParticipantId.Equals(participantId))
                .ToListAsync();
        }

        public async Task<IEnumerable<OuraSleep>> GetOuraSleepsByDateRangeAsync(
            DateTime fromDate, DateTime toDate) {
            return await FindByCondition(p => p.SummaryDate.CompareTo(fromDate) >= 0 &&
                                              p.SummaryDate.CompareTo(toDate) < 0)
                .ToListAsync();
        }

        public async Task<OuraSleep> GetOuraSleepByParticipantIdAndDateAsync(int participantId, DateTime date) {
            return await FindByCondition(p => p.SummaryDate.CompareTo(date) == 0 &&
                                              p.ParticipantId.Equals(participantId))
                .DefaultIfEmpty(new OuraSleep())
                .SingleAsync();
        }

        public void CreateOuraSleep(OuraSleep ouraSleep) {
            Create(ouraSleep);
        }

        public void DeleteOuraSleep(OuraSleep ouraSleep) {
            Delete(ouraSleep);
        }
        
        public void UpdateOuraSleep(OuraSleep dbOuraSleep, OuraSleep ouraSleep) {
            dbOuraSleep.Map(ouraSleep);
            Update(dbOuraSleep);
        }
    }
}