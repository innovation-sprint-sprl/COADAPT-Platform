using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repository.ModelRepository {
    public interface IAppUsageLogRepository : IRepositoryBase<AppUsageLog> {
        Task<IEnumerable<AppUsageLog>> GetAllAppUsageLogAsync();
        Task<AppUsageLog> GetAppUsageLogByIdAsync(int id);
        Task<IEnumerable<AppUsageLog>> GetAllAppUsageLogByUserIdAsync(int userId);
        Task<IEnumerable<AppUsageLog>> GetAllAppUsageLogAfterDateAsync(DateTime date);
        Task<IEnumerable<AppUsageLog>> GetAllAppUsageLogAfterDateByUserIdAsync(DateTime date, int userId);
        void CreateAppUsageLog(AppUsageLog appUsageLog);
        void DeleteAppUsageLog(AppUsageLog appUsageLog);
    }
}
