using Entities.Conferences;
using Microsoft.AspNetCore.Mvc;
using UseCases.Interfaces;

namespace Banks.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConferenceController : ApplicationController
{
    private readonly IDb _db;

    public ConferenceController(IDb db)
    {
        _db = db;
    }

    [Route("Search")]
    [HttpGet]
    public IActionResult Search(string key)
    {
        if (key.Length < 8)
            return BadRequest("key is very short");
        var result = _db.Query<Conference>().FilterByKeyword(key).ToList();
        return Ok(result);
    }

    [Route("FindByTitle")]
    [HttpGet]
    public IActionResult FindByTitle(string title)
    {
        try
        {
            var result = _db.Query<Conference>().FilterByTitle(title);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}