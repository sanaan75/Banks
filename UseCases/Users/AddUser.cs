using Entities.Helpers;
using Entities.Users;
using Framework;
using System.Linq;

namespace UseCases.Users
{
    public class AddUser : IAddUser
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddUser(IUnitOfWork unitOfWork)
        {            
            _unitOfWork = unitOfWork;
        }

        public User Responce(IAddUser.Request request)
        {
            var userExist = _unitOfWork.Users.Find(i => i.Username == request.UserName).SingleOrDefault();

            Check.Null(userExist, () => "کاربر قبلا تعریف شده است");

            return _unitOfWork.Users.Add(new User
            {
                Title = request.Title,
                Username = request.UserName,
                Password = HashPassword.Hash(request.UserName, request.Password),
                Enabled = request.Enabled,
                SysAdmin = request.SysAdmin
            });          

        }
    }
}
