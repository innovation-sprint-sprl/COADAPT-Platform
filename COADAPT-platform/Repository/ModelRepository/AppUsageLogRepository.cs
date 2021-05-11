using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.ModelRepository {
    public class AppUsageLogRepository : RepositoryBase<AppUsageLog>, IAppUsageLogRepository {
        public AppUsageLogRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

        public void CreateAppUsageLog(AppUsageLog appUsageLog) {
            Create(appUsageLog);
        }

        public void DeleteAppUsageLog(AppUsageLog appUsageLog) {
            Delete(appUsageLog);
        }

        public async Task<IEnumerable<AppUsageLog>> GetAllAppUsageLogAfterDateAsync(DateTime date) {
            return await FindByCondition(p => p.ReportedOn.CompareTo(date) >= 0)
                .ToListAsync();
        }

        public async Task<IEnumerable<AppUsageLog>> GetAllAppUsageLogAfterDateByUserIdAsync(DateTime date, int userId) {
            return await FindByCondition(p => p.ReportedOn.CompareTo(date) >= 0 && p.UserId.Equals(userId))
                .ToListAsync();
        }

        public async Task<IEnumerable<AppUsageLog>> GetAllAppUsageLogAsync() {
            return await FindAll().ToListAsync();
        }

        public async Task<IEnumerable<AppUsageLog>> GetAllAppUsageLogByUserIdAsync(int userId) {
            return await FindByCondition(p => p.UserId.Equals(userId))
                .ToListAsync();
        }

        public async Task<AppUsageLog> GetAppUsageLogByIdAsync(int id) {
            return await FindByCondition(p => p.Id == id)
                .DefaultIfEmpty(new AppUsageLog())
                .SingleAsync();
        }
    }
}
