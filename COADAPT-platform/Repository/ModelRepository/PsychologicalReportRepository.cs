using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Extensions;

namespace Repository.ModelRepository {
    public class PsychologicalReportRepository : RepositoryBase<PsychologicalReport>, IPsychologicalReportRepository {
        public PsychologicalReportRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

        public void CreatePsychologicalReport(PsychologicalReport psychologicalReport) {
            Create(psychologicalReport);
        }

        public void DeletePsychologicalReport(PsychologicalReport psychologicalReport) {
            Delete(psychologicalReport);
        }
        
        public void UpdatePsychologicalReport(PsychologicalReport dbPsychologicalReport, PsychologicalReport psychologicalReport) {
            dbPsychologicalReport.Map(psychologicalReport);
            Update(dbPsychologicalReport);
        }

        public async Task<IEnumerable<PsychologicalReport>> GetPsychologicalReportsAfterDateAsync(DateTime date) {
            return await FindByCondition(p => p.DateOfReport.CompareTo(date) >= 0)
                .Include(p => p.Participant)
                .ToListAsync();
        }

        public async Task<IEnumerable<PsychologicalReport>> GetPsychologicalReportsAfterDateByParticipantIdAsync(DateTime date, int participantId) {
            return await FindByCondition(p => p.DateOfReport.CompareTo(date) >= 0 && p.ParticipantId.Equals(participantId))
                .OrderByDescending(p => p.DateOfReport)
                .Include(p => p.Participant)
                .ToListAsync();
        }

        public async Task<IEnumerable<PsychologicalReport>> GetPsychologicalReportsByParticipantIdAndDateRangeAsync(
            int participantId, DateTime fromDate, DateTime toDate) {
            return await FindByCondition(p => p.DateOfReport.CompareTo(fromDate) >= 0 &&
                                              p.DateOfReport.CompareTo(toDate) < 0 &&
                                              p.ParticipantId.Equals(participantId))
                .ToListAsync();
        }

        public async Task<PsychologicalReport> GetPsychologicalReportByParticipantIdAndDateAsync(int participantId, DateTime date) {
            return await FindByCondition(p => p.DateOfReport.CompareTo(date) == 0 &&
                                              p.ParticipantId.Equals(participantId))
                .DefaultIfEmpty(new PsychologicalReport())
                .SingleAsync();
        }

        public async Task<IEnumerable<PsychologicalReport>> GetPsychologicalReportsAsync() {
            return await FindAll().Include(p => p.Participant).ToListAsync();
        }

        public async Task<IEnumerable<PsychologicalReport>> GetPsychologicalReportsByParticipantIdAsync(int participantId) {
            return await FindByCondition(p => p.ParticipantId.Equals(participantId))
                .OrderByDescending(p => p.DateOfReport)
                .Include(p => p.Participant)
                .ToListAsync();
        }

        public async Task<PsychologicalReport> GetPsychologicalReportByIdAsync(int id) {
            return await FindByCondition(p => p.Id == id)
                .DefaultIfEmpty(new PsychologicalReport())
                .SingleAsync();
        }
    }
}
