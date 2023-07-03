using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UseCases.Interfaces;
using UseCases.Users;
using Web;

namespace Banks.Pages._App.Users;

public class Create : PageModel
{
    private readonly IAddUser _addUser;
    private readonly IDb _db;
    public AddUserModel UserModel;

    public Create(IAddUser addUser, IDb db)
    {
        _addUser = addUser;
        _db = db;
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

            _db.Save();

            return new RedirectResult(PageUrl.Users);
        }
        catch (Exception ex)
        {
            ViewData["error"] = ex.Message;
            return Page();
        }
    }
}

public class AddUserModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Title { get; set; }
    public bool Enabled { get; set; }
    public bool SysAdmin { get; set; }
}