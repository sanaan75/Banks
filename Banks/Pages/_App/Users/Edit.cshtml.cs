using Entities.Users;
using Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using UseCases.Users;

namespace JournalBank.Pages._App.Users
{
    public class Edit : PageModel
    {
        public User User { get; set; }
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEditUser _editUser;

        public Edit(IEditUser editUser, IUnitOfWork unitOfWork)
        {
            _editUser = editUser;
            _unitOfWork = unitOfWork;
        }


        public ActionResult OnGet(int id)
        {
            if (id == 0)
                return NotFound();


            User = _unitOfWork.Users.GetById(id);

            if (User == null)
            {
                return NotFound();
            }

            return Page();
        }

        public ActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (User != null)
            {
                try
                {
                    _editUser.Responce(new IEditUser.Request
                    {
                        Password = "1234",
                        PasswordConfirm = "1234",
                        Title = "111"
                    });
                    _unitOfWork.Save();
                }
                catch (Exception ex)
                {
                    ViewData["error"] = ex.Message;
                    return Page();
                }
            }

            return RedirectToPage("./Index");
        }
    }
}