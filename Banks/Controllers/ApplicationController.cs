using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Banks.Controllers;

[ApiController]
public class ApplicationController : Controller
{
    public string? Token { get; set; }

    public override void OnActionExecuting(ActionExecutingContext ctx)
    {
        // StringValues headerValue;
        // Request.Headers.TryGetValue("JiroToken", out headerValue);
        // var headerValueResult = headerValue.FirstOrDefault();
        // Console.WriteLine(headerValueResult);
        //
        // if (headerValueResult.Equals(AppSetting.Api_Key) == false)
        //     throw new AppException(401, "");

        Token = HttpContext?.User?.Claims.FirstOrDefault() != null
            ? HttpContext.User.Claims.First().Value
            : string.Empty;
    }
}