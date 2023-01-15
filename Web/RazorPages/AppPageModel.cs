using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.RazorPages
{
    [DisableRequestSizeLimit]
    public class AppPageModel : PageModel
    {
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            if (HttpContext.Session.GetActor() == null)
            {
                context.Result = new RedirectResult(PageUrl.Login);
                return;
            }

            if (HttpContext.Session.GetActor().IsAuthenticated == false)
                context.Result = new RedirectResult(PageUrl.Login);

            base.OnPageHandlerExecuting(context);
        }
    }
}