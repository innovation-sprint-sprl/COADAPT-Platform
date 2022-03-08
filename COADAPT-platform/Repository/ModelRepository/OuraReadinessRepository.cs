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
    public class OuraReadinessRepository : RepositoryBase<OuraReadiness>, IOuraReadinessRepository {
        public OuraReadinessRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

        public async Task<IEnumerable<OuraReadiness>> GetOuraReadinessesAsync() {
            return await FindAll().ToListAsync();
        }

        public async Task<OuraReadiness> GetOuraReadinessByIdAsync(int id) {
            return await FindByCondition(p => p.Id == id)
                .DefaultIfEmpty(new OuraReadiness())
                .SingleAsync();
        }

        public bool Exists(int participantId, DateTime summaryDate) {
            return FindByCondition(x => x.ParticipantId == participantId && x.SummaryDate == summaryDate).Any();
        }

        public async Task<IEnumerable<OuraReadiness>> GetOuraReadinessesByParticipantIdAsync(int participantId) {
            return await FindByCondition(p => p.ParticipantId.Equals(participantId))
                .ToListAsync();
        }

        public async Task<IEnumerable<OuraReadiness>> GetOuraReadinessesByParticipantIdAndDateRangeAsync(
            int participantId, DateTime fromDate, DateTime toDate) {
            return await FindByCondition(p => p.SummaryDate.CompareTo(fromDate) >= 0 &&
                                              p.SummaryDate.CompareTo(toDate) < 0 &&
                                              p.ParticipantId.Equals(participantId))
                .ToListAsync();
        }

        public async Task<IEnumerable<OuraReadiness>> GetOuraReadinessesByDateRangeAsync(
            DateTime fromDate, DateTime toDate) {
            return await FindByCondition(p => p.SummaryDate.CompareTo(fromDate) >= 0 &&
                                              p.SummaryDate.CompareTo(toDate) < 0)
                .ToListAsync();
        }

        public async Task<OuraReadiness> GetOuraReadinessByParticipantIdAndDateAsync(int participantId, DateTime date) {
            return await FindByCondition(p => p.SummaryDate.CompareTo(date) == 0 &&
                                              p.ParticipantId.Equals(participantId))
                .DefaultIfEmpty(new OuraReadiness())
                .SingleAsync();
        }

        public void CreateOuraReadiness(OuraReadiness ouraReadiness) {
            Create(ouraReadiness);
        }

        public void DeleteOuraReadiness(OuraReadiness ouraReadiness) {
            Delete(ouraReadiness);
        }
        
        public void UpdateOuraReadiness(OuraReadiness dbOuraReadiness, OuraReadiness ouraReadiness) {
            dbOuraReadiness.Map(ouraReadiness);
            Update(dbOuraReadiness);
        }
    }
}