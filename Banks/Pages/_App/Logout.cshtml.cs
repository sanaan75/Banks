using Microsoft.AspNetCore.Mvc;
using Web;
using Web.RazorPages;

namespace Banks.Pages._App;

public class LogoutModel : AppPageModel
{
    public RedirectResult OnGet()
    {
        HttpContext.Session.LogoutActor();
        return new RedirectResult(PageUrl.Login);
    }
}