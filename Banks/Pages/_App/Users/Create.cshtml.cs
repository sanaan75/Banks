using System;
using Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UseCases.Users;
using Web;
using Web.Models.Users;

namespace JournalBank.Pages._App.Users
{
    public class Create : PageModel
    {
        private readonly IAddUser _addUser;
        private readonly IUnitOfWork _unitOfWork;
        public AddUserModel UserModel;

        public Create(IAddUser addUser, IUnitOfWork unitOfWork)
        {
            _addUser = addUser;
            _unitOfWork = unitOfWork;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost(AddUserModel userModel)
        {
            try
            {
                _addUser.Responce(new IAddUser.Request
                {
                    UserName = userModel.UserName,
                    Password = userModel.Password,
                    Title = userModel.Title,
                    Enabled = userModel.Enabled,
                    SysAdmin = userModel.SysAdmin
                });

                _unitOfWork.Save();

                return new RedirectResult(PageUrl.Users);
            }
            catch (Exception ex)
            {
                ViewData["error"] = ex.Message;
                return Page();
            }
        }
    }
}
