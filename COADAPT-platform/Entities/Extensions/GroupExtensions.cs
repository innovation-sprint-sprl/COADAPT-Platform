using Entities.Models;

namespace Entities.Extensions {
    public static class GroupExtensions {
        public static void Map(this Group dbGroup, Group group) {
            dbGroup.Name = group.Name;
            dbGroup.Shortname = group.Shortname;
            dbGroup.StudyId = group.StudyId;
        }
    }
}