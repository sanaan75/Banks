using Entities;
using Entities.Helpers;
using Entities.Permissions;
using Entities.Users;
using UseCases.Interfaces;

namespace UseCases;

public class Login : ILogin
{
    private readonly IActorService _actorService;
    private readonly IDb _db;

    public Login(IDb db, IActorService actorService)
    {
        _db = db;
        _actorService = actorService;
    }

    public Actor Respond(string username, string password)
    {
        try
        {
            var users = _db.Query<User>().ToList();
            var user = _db.Query<User>().Where(i => i.Username == username).FirstOrDefault(i => i.SysAdmin == true);
            if (user != null)
            {
                var hashedPass = HashPassword.Hash(username, password);
                if (hashedPass.Equals(user.Password))
                {
                    var permissions = _db.Query<UserGroupPermission>()
                        .Where(i => i.UserGroupId == user.UserGroupId)
                        .Select(i => i.Permission).ToList();

                    var actor = new Actor
                    {
                        IsAuthenticated = true,
                        UserId = user.Id,
                        IsSysAdmin = user.SysAdmin,
                        Permissions = permissions,
                        FullName = user.Title
                    };

                    _actorService.SetActor(actor);
                    return actor;
                }
            }
        }
        catch (Exception ex)
        {
            //ignored
        }

        return new Actor
        {
            IsAuthenticated = false
        };
    }

    public Actor GetActor()
    {
        try
        {
            if (_actorService.IsAuthenticated)
                return _actorService.GetActor();
        }
        catch
        {
            // ignored
        }

        return null;
    }
}