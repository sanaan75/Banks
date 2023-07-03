using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web;

namespace Banks.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public RedirectToPageResult OnGet()
    {
        var actor = HttpContext.Session.GetActor();
        if (actor.IsAuthenticated)
            return RedirectToPage(PageUrl.Home);

        return RedirectToPage(PageUrl.Login);
    }
}