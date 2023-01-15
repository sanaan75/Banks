using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UseCases;
using Web;

namespace Banks.Pages;

public class Login : PageModel
{
    private readonly ILogin _login;

    public Login(ILogin login)
    {
        _login = login;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost(string username, string password)
    {
        try
        {
            var actor = _login.Respond(username, password);
            HttpContext.Session.SetActor(actor);
            return new RedirectResult(PageUrl.Home);
        }
        catch (Exception e)
        {
            ViewData["error"] = e.Message;
            return Page();
        }
    }
}