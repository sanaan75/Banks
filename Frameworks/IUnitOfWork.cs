using System;
using Frameworks;

namespace Framework
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IJournalRepository Journals { get; }
        IJournalRecordRepository JournalRecords { get; }
        IFileRepository Files { get; }
        IFileContentRepository FileContents { get; }

        IUserInGroupRepository UserInGroups { get; }
        IUserGroupRepository UserGroups { get; }
        IUserGroupPermissionRepository UserGroupPermissions { get; }
        int Save();
    }
}