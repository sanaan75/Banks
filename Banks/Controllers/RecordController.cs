using Entities.Journals;
using Entities.Utilities;
using Microsoft.AspNetCore.Mvc;
using UseCases.Interfaces;
using Web.Models;

namespace Banks.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecordController : ApplicationController
{
    private readonly IDb _db;

    public RecordController(IDb db)
    {
        _db = db;
    }

    [Route("GetBestRank")]
    [HttpGet]
    public IActionResult GetISCList(int year, JournalIndex index, string title)
    {
        try
        {
            var item = _db.Query<JournalRecord>()
                .FilterByIndex(index)
                .Where(k => k.Year < year)
                .Where(i => i.Journal.Title.Trim().ToLower().Equals(title.Trim().ToLower()))
                .OrderBy(k => k.QRank)
                .Select(i => new RecordModel
                {
                    Category = i.Category,
                    QRank = i.QRank!.GetCaption(),
                    Year = i.Year,
                    If = i.If,
                    Type = i.Type!.GetCaption()
                })
                .FirstOrDefault();

            return Ok(item);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}