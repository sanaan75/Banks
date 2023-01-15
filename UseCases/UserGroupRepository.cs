using Entities.Permissions;
using Framework;

namespace Services.Repositories;

public class UserGroupRepository : BaseRepository<Persistence.AppContext, UserGroup>, IUserGroupRepository
{
    public UserGroupRepository(Persistence.AppContext appContext)
        : base(appContext)
    {
    }
}