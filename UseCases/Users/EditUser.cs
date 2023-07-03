using Entities.Users;
using Entities.Utilities;
using UseCases.Interfaces;

namespace UseCases.Users;

public class EditUser : IEditUser
{
    private readonly IDb _db;

    public EditUser(IDb uitOfWork)
    {
        _db = uitOfWork;
    }

    public void Responce(IEditUser.Request request)
    {
        var user = _db.Query<User>().GetById(request.Id);

        Check.NotNull(user, () => "کاربر یافت نشد");

        user.Title = request.Title;
        user.Enabled = request.Enabled;
        user.SysAdmin = request.SysAdmin;

        var passwordShouldBeUpdated = string.IsNullOrWhiteSpace(request.Password) == false;

        if (passwordShouldBeUpdated)
        {
            Check.Equal(request.Password, request.PasswordConfirm, () => "PasswordNotMatch");
            var newPasswordHashed = HashPassword.Hash(user.Username, request.Password);
            Check.NotEqual(newPasswordHashed, user.Password, () => "PasswordNotChanged");

            user.Password = newPasswordHashed;
        }
    }
}