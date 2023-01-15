using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Entities;

public class ApiException : Exception
{
    public static StatusCodeResult ApiError(HttpStatusCode code)
    {
        return new StatusCodeResult((int)code);
    }
}