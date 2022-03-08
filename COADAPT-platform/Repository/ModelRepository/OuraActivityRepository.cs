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
    public class OuraActivityRepository : RepositoryBase<OuraActivity>, IOuraActivityRepository {
        public OuraActivityRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

        public async Task<IEnumerable<OuraActivity>> GetOuraActivitiesAsync() {
            return await FindAll().ToListAsync();
        }

        public async Task<OuraActivity> GetOuraActivityByIdAsync(int id) {
            return await FindByCondition(p => p.Id == id)
                .DefaultIfEmpty(new OuraActivity())
                .SingleAsync();
        }

        public bool Exists(int participantId, DateTime summaryDate) {
            return FindByCondition(x => x.ParticipantId == participantId && x.SummaryDate == summaryDate).Any();
        }

        public async Task<IEnumerable<OuraActivity>> GetOuraActivitiesByParticipantIdAsync(int participantId) {
            return await FindByCondition(p => p.ParticipantId.Equals(participantId))
                .ToListAsync();
        }

        public async Task<IEnumerable<OuraActivity>> GetOuraActivitiesByParticipantIdAndDateRangeAsync(
            int participantId, DateTime fromDate, DateTime toDate) {
            return await FindByCondition(p => p.SummaryDate.CompareTo(fromDate) >= 0 &&
                                              p.SummaryDate.CompareTo(toDate) < 0 &&
                                              p.ParticipantId.Equals(participantId))
                .ToListAsync();
        }

        public async Task<IEnumerable<OuraActivity>> GetOuraActivitiesByDateRangeAsync(
            DateTime fromDate, DateTime toDate) {
            return await FindByCondition(p => p.SummaryDate.CompareTo(fromDate) >= 0 &&
                                              p.SummaryDate.CompareTo(toDate) < 0)
                .ToListAsync();
        }

        public async Task<OuraActivity> GetOuraActivityByParticipantIdAndDateAsync(int participantId, DateTime date) {
            return await FindByCondition(p => p.SummaryDate.CompareTo(date) == 0 &&
                                              p.ParticipantId.Equals(participantId))
                .DefaultIfEmpty(new OuraActivity())
                .SingleAsync();
        }

        public void CreateOuraActivity(OuraActivity ouraActivity) {
            Create(ouraActivity);
        }

        public void DeleteOuraActivity(OuraActivity ouraActivity) {
            Delete(ouraActivity);
        }
        
        public void UpdateOuraActivity(OuraActivity dbOuraActivity, OuraActivity ouraActivity) {
            dbOuraActivity.Map(ouraActivity);
            Update(dbOuraActivity);
        }
    }
}