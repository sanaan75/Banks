using Entities.Permissions;
using Framework;
using Frameworks;
using Persistence;
using Services.Repositories;
using UseCases.Journals;
using UseCases.Users;
using AppContext = Persistence.AppContext;

namespace UseCases
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppContext _appContext;
        private readonly FileContext _fileContext;

        //---------------------------------------
        private IUserRepository _userRepository;
        private IFileRepository _fileRepository;
        private IFileContentRepository _fileContentRepository;
        private IJournalRepository _journalRepository;
        private IJournalRecordRepository _journalRecordRepository;
        private IUserGroupRepository _userGroupRepository;
        private IUserInGroupRepository _userInGroupRepository;
        private IUserGroupPermissionRepository _userGroupPermissionRepository;

        public UnitOfWork(AppContext appContext, FileContext fileContext)
        {
            _appContext = appContext;
            _fileContext = fileContext;
        }

        public IUserRepository Users
        {
            get
            {
                if (_userRepository is null)
                    _userRepository = new UserRepository(_appContext);

                return _userRepository;
            }
        }

        public IJournalRepository Journals
        {
            get
            {
                if (_journalRepository is null)
                    _journalRepository = new JournalRepository(_appContext);

                return _journalRepository;
            }
        }


        public IJournalRecordRepository JournalRecords
        {
            get
            {
                if (_journalRecordRepository is null)
                    _journalRecordRepository = new JournalRecordRepository(_appContext);

                return _journalRecordRepository;
            }
        }

        public IFileRepository Files
        {
            get
            {
                if (_fileRepository is null)
                    _fileRepository = new FileRepository(_fileContext);

                return _fileRepository;
            }
        }

        public IFileContentRepository FileContents
        {
            get
            {
                if (_fileContentRepository is null)
                    _fileContentRepository = new FileContentRepository(_fileContext);

                return _fileContentRepository;
            }
        }

        public IUserGroupRepository UserGroups
        {
            get
            {
                if (_userGroupRepository is null)
                    _userGroupRepository = new UserGroupRepository(_appContext);

                return _userGroupRepository;
            }
        }
        
        public IUserInGroupRepository UserInGroups
        {
            get
            {
                if (_userInGroupRepository is null)
                    _userInGroupRepository = new UserInGroupRepository(_appContext);

                return _userInGroupRepository;
            }
        }
        
        public IUserGroupPermissionRepository UserGroupPermissions
        {
            get
            {
                if (_userGroupPermissionRepository is null)
                    _userGroupPermissionRepository = new UserGroupPermissionRepository(_appContext);

                return _userGroupPermissionRepository;
            }
        }

        public void Dispose()
        {
            _appContext.Dispose();
        }

        public int Save()
        {
            if (_appContext.ChangeTracker.HasChanges())
                return _appContext.SaveChanges();

            return _fileContext.SaveChanges();
        }

        public int SaveFile()
        {
            return _fileContext.SaveChanges();
        }
    }
}