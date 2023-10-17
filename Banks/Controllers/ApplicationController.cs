using Entities;
using Entities.Utilities.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Banks.Controllers;

[ApiController]
public class ApplicationController : Controller
{
    public string? Token { get; set; }

    public override void OnActionExecuting(ActionExecutingContext ctx)
    {
        StringValues headerValue;
        Request.Headers.TryGetValue("JiroToken", out headerValue);
        var headerValueResult = headerValue.FirstOrDefault();
        Console.WriteLine(headerValueResult);

        if (headerValueResult == null)
        {
            ctx.HttpContext.Response.Clear();
            ctx.HttpContext.Response.ContentType = "application/json";
            ctx.HttpContext.Response.StatusCode = 401;
            ctx.Result = StatusCode(401,
                new
                {
                    Message = "Invalid Request"
                });
            return;
        }
        
        if (headerValueResult!.Equals(AppSetting.Api_Key) == false)
        {
            ctx.HttpContext.Response.Clear();
            ctx.HttpContext.Response.ContentType = "application/json";
            ctx.HttpContext.Response.StatusCode = 401;
            ctx.Result = StatusCode(401,
                new
                {
                    Message =
                        $"Invalid Request"
                });
            return;
        }

        // Token = HttpContext?.User?.Claims.FirstOrDefault() != null
        //     ? HttpContext.User.Claims.First().Value
        //     : string.Empty;
    }
}