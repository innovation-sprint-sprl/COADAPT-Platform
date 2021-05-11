using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repository.ModelRepository {
    public interface IPsychologicalReportRepository : IRepositoryBase<PsychologicalReport> {
        Task<IEnumerable<PsychologicalReport>> GetPsychologicalReportsAsync();
        Task<PsychologicalReport> GetPsychologicalReportByIdAsync(int id);
        Task<IEnumerable<PsychologicalReport>> GetPsychologicalReportsByParticipantIdAsync(int participantId);
        Task<IEnumerable<PsychologicalReport>> GetPsychologicalReportsAfterDateAsync(DateTime date);
        Task<IEnumerable<PsychologicalReport>> GetPsychologicalReportsAfterDateByParticipantIdAsync(DateTime date, int participantId);
        Task<IEnumerable<PsychologicalReport>> GetPsychologicalReportsByParticipantIdAndDateRangeAsync(
            int participantId, DateTime fromDate, DateTime toDate);
        Task<PsychologicalReport> GetPsychologicalReportByParticipantIdAndDateAsync(int participantId, DateTime date);
        void CreatePsychologicalReport(PsychologicalReport psychologicalReport);
        void DeletePsychologicalReport(PsychologicalReport psychologicalReport);
        void UpdatePsychologicalReport(PsychologicalReport dbPsychologicalReport, PsychologicalReport psychologicalReport);
    }
}
