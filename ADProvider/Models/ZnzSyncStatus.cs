using System;

namespace ADProvider.Models
{
    [Flags]
    public enum ZnzSyncStatus
    {
        Unknown = 0,
        GroupNext = 1,
        GroupUpdate = 2,
        NameFix = 4,
        InSync = 8,
        Create = 16,
        NotFound = 32,
    }
}
