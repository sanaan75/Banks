using Entities.Permissions;
using Framework;

namespace Services.Repositories;

public class UserGroupPermissionRepository : BaseRepository<Persistence.AppContext, UserGroupPermission>, IUserGroupPermissionRepository
{
    public UserGroupPermissionRepository(Persistence.AppContext appContext)
        : base(appContext)
    {
    }
}