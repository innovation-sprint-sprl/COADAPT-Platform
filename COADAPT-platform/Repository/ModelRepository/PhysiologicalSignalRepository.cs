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
    public class PhysiologicalSignalRepository : RepositoryBase<PhysiologicalSignal>, IPhysiologicalSignalRepository {
        public PhysiologicalSignalRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

        public async Task<IEnumerable<PhysiologicalSignal>> GetPhysiologicalSignalsAsync() {
            return await FindAll().ToListAsync();
        }

        public async Task<PhysiologicalSignal> GetPhysiologicalSignalByIdAsync(int id) {
            return await FindByCondition(p => p.Id == id)
                .DefaultIfEmpty(new PhysiologicalSignal())
                .SingleAsync();
        }

        public bool Exists(int participantId, DateTime timestamp) {
            return FindByCondition(x => x.ParticipantId == participantId && x.Timestamp == timestamp).Any();
        }

        public async Task<IEnumerable<PhysiologicalSignal>> GetPhysiologicalSignalsByParticipantIdAsync(int participantId) {
            return await FindByCondition(p => p.ParticipantId.Equals(participantId))
                .ToListAsync();
        }

        public async Task<IEnumerable<PhysiologicalSignal>> GetPhysiologicalSignalsAfterDateByParticipantIdAsync(DateTime fromDate, int participantId) {
            return await FindByCondition(p => p.Timestamp.CompareTo(fromDate) >= 0 && p.ParticipantId.Equals(participantId))
                .ToListAsync();
        }

        public async Task<IEnumerable<PhysiologicalSignal>> GetPhysiologicalSignalsInDateRangeByParticipantIdAsync(DateTime fromDate, DateTime toDate, int participantId) {
            return await FindByCondition(p => p.Timestamp.CompareTo(fromDate) >= 0 && p.Timestamp.CompareTo(toDate) <= 0 && p.ParticipantId.Equals(participantId))
                .ToListAsync();
        }

        public async Task<IEnumerable<PhysiologicalSignal>> GetPhysiologicalSignalsAfterDateAsync(DateTime date) {
            return await FindByCondition(p => p.Timestamp.CompareTo(date) >= 0)
                .ToListAsync();
        }

        public async Task<IEnumerable<PhysiologicalSignal>> GetPhysiologicalSignalsByParticipantIdAndTypeAsync(
            int participantId, string type) {
            return await FindByCondition(p => p.ParticipantId.Equals(participantId) &&
                                              p.Type == type)
                .ToListAsync();
        }

        public async Task<IEnumerable<PhysiologicalSignal>> GetPhysiologicalSignalsByParticipantIdAndTypeAndDateRangeAsync(
            int participantId, string type, DateTime fromDate, DateTime toDate) {
            return await FindByCondition(p => p.Timestamp.CompareTo(fromDate) >= 0 &&
                                              p.Timestamp.CompareTo(toDate) < 0 && p.Type == type &&
                                              p.ParticipantId.Equals(participantId))
                .ToListAsync();
        }

        public async Task<PhysiologicalSignal> GetPhysiologicalSignalByParticipantIdAndTypeAndDateAsync(
            int participantId, string type, DateTime date) {
            return await FindByCondition(p => p.Timestamp.CompareTo(date) == 0 &&
                                              p.Type == type && p.ParticipantId.Equals(participantId))
                .DefaultIfEmpty(new PhysiologicalSignal())
                .SingleAsync();
        }

        public void CreatePhysiologicalSignal(PhysiologicalSignal physiologicalSignal) {
            Create(physiologicalSignal);
        }

        public void DeletePhysiologicalSignal(PhysiologicalSignal physiologicalSignal) {
            Delete(physiologicalSignal);
        }
        
        public void UpdatePhysiologicalSignal(PhysiologicalSignal dbPhysiologicalSignal, PhysiologicalSignal physiologicalSignal) {
            dbPhysiologicalSignal.Map(physiologicalSignal);
            Update(dbPhysiologicalSignal);
        }

    }
}