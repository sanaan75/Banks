using Entities.Users;
using Framework;
using Services;
using AppContext = Persistence.AppContext;

namespace UseCases.Users
{
    public class UserRepository : BaseRepository<AppContext, User>, IUserRepository
    {
        public UserRepository(AppContext appContext)
            : base(appContext)
        {
        }
    }
}