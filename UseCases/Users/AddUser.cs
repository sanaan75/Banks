using Entities.Helpers;
using Entities.Users;
using UseCases.Interfaces;

namespace UseCases.Users;

public class AddUser : IAddUser
{
    private readonly IDb _db;

    public AddUser(IDb db)
    {
        _db = db;
    }

    public User Responce(IAddUser.Request request)
    {
        var userExist = _db.Query<User>().SingleOrDefault(i => i.Username == request.UserName);

        Check.Null(userExist, () => "کاربر قبلا تعریف شده است");

        return _db.Set<User>().Add(new User
        {
            Title = request.Title,
            Username = request.UserName,
            Password = HashPassword.Hash(request.UserName, request.Password),
            Enabled = request.Enabled,
            SysAdmin = request.SysAdmin
        }).Entity;
    }
}