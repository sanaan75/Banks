using Entities.Permissions;
using Framework;

namespace Services.Repositories;

public class UserInGroupRepository : BaseRepository<Persistence.AppContext, UserInGroup>, IUserInGroupRepository
{
    public UserInGroupRepository(Persistence.AppContext appContext)
        : base(appContext)
    {
    }
}