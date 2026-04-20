using StudyHub.Domain.Entities;

namespace StudyHub.Core.Common;

public static class StorageScopeHelper
{
    public static string? ResolveContainerName(User user, string scope)
    {
        if (string.Equals(scope, "group", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(user.Group?.Name))
            {
                return null;
            }

            return StorageNamingHelper.BuildGroupStorageContainerName(user.Group.Name);
        }

        return StorageNamingHelper.BuildUserStorageContainerName(user.Id);
    }
}