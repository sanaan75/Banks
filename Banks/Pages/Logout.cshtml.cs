using Microsoft.AspNetCore.Mvc;
using Web;
using Web.RazorPages;

namespace JournalBank.Pages;

public class LogoutModel : AppPageModel
{
    public RedirectResult OnGet()
    {
        HttpContext.Session.LogoutActor();
        return new RedirectResult(PageUrl.Login);
    }
}