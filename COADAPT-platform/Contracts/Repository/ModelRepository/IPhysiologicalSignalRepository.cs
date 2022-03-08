using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repository.ModelRepository {
    public interface IPhysiologicalSignalRepository : IRepositoryBase<PhysiologicalSignal> {
        Task<IEnumerable<PhysiologicalSignal>> GetPhysiologicalSignalsAsync();
        Task<PhysiologicalSignal> GetPhysiologicalSignalByIdAsync(int id);
        bool Exists(int participantId, DateTime timestamp);
        Task<IEnumerable<PhysiologicalSignal>> GetPhysiologicalSignalsByParticipantIdAsync(int participantId);
        Task<IEnumerable<PhysiologicalSignal>> GetPhysiologicalSignalsAfterDateByParticipantIdAsync(DateTime fromDate, int participantId);
        Task<IEnumerable<PhysiologicalSignal>> GetPhysiologicalSignalsInDateRangeByParticipantIdAsync(DateTime fromDate, DateTime toDate, int participantId);
        Task<IEnumerable<PhysiologicalSignal>> GetPhysiologicalSignalsAfterDateAsync(DateTime date);
        Task<IEnumerable<PhysiologicalSignal>> GetPhysiologicalSignalsByParticipantIdAndTypeAsync(int participantId, string type);
        Task<IEnumerable<PhysiologicalSignal>> GetPhysiologicalSignalsByParticipantIdAndTypeAndDateRangeAsync(
            int participantId, string type, DateTime fromDate, DateTime toDate);
        Task<PhysiologicalSignal> GetPhysiologicalSignalByParticipantIdAndTypeAndDateAsync(int participantId, string type, DateTime date);
        void CreatePhysiologicalSignal(PhysiologicalSignal physiologicalSignal);
        void DeletePhysiologicalSignal(PhysiologicalSignal physiologicalSignal);
        void UpdatePhysiologicalSignal(PhysiologicalSignal dbPhysiologicalSignal, PhysiologicalSignal physiologicalSignal);
    }
}
