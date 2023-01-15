using Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Helpers;
using Framework;

namespace UseCases.Users
{
    public class EditUser : IEditUser
    {
        
        private readonly IUnitOfWork _unitOfWork;

        public EditUser(IUnitOfWork uitOfWork)
        {
            _unitOfWork = uitOfWork;
        }

        public void Responce(IEditUser.Request request)
        {
            var user = _unitOfWork.Users.GetById(request.Id);

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
}
