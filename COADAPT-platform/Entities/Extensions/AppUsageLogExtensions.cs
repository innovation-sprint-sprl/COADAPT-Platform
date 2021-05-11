using Entities.Models;

namespace Entities.Extensions {
    public static class AppUsageLogExtensions {
        public static void Map(this AppUsageLog dbAppUsageLog, AppUsageLog appUsageLog) {
            dbAppUsageLog.ReportedOn = appUsageLog.ReportedOn;
            dbAppUsageLog.UserId = appUsageLog.UserId;
            dbAppUsageLog.Tag = appUsageLog.Tag;
            dbAppUsageLog.Message = appUsageLog.Message;
        }
    }
}
