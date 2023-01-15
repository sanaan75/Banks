using Entities;
using Entities.Helpers;
using Framework;
using System.Linq;

namespace UseCases
{
    public class Login : ILogin
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActorService _actorService;
        
        public Login(IUnitOfWork unitOfWork, IActorService actorService)
        {
            _unitOfWork = unitOfWork;
            _actorService = actorService;
        }

        public Actor Respond(string username, string password)
        {
            try
            {
                var users = _unitOfWork.Users.GetAll().ToList();
                var user = _unitOfWork.Users.Find(i => i.Username == username).FirstOrDefault(i => i.SysAdmin == true);
                if (user != null)
                {
                    var hashedPass = HashPassword.Hash(username, password);
                    if (hashedPass.Equals(user.Password))
                    {
                        var permissions = _unitOfWork.UserGroupPermissions.GetAll()
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
}