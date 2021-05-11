using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repository.ModelRepository {
    public interface IOuraReadinessRepository : IRepositoryBase<OuraReadiness> {
        Task<IEnumerable<OuraReadiness>> GetOuraReadinessesAsync();
        Task<OuraReadiness> GetOuraReadinessByIdAsync(int id);
        Task<IEnumerable<OuraReadiness>> GetOuraReadinessesByParticipantIdAsync(int participantId);
        Task<IEnumerable<OuraReadiness>> GetOuraReadinessesByParticipantIdAndDateRangeAsync(
            int participantId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<OuraReadiness>> GetOuraReadinessesByDateRangeAsync(
            DateTime fromDate, DateTime toDate);
        Task<OuraReadiness> GetOuraReadinessByParticipantIdAndDateAsync(int participantId, DateTime date);
        void CreateOuraReadiness(OuraReadiness ouraReadiness);
        void DeleteOuraReadiness(OuraReadiness ouraReadiness);
        void UpdateOuraReadiness(OuraReadiness dbOuraReadiness, OuraReadiness ouraReadiness);
    }
}
