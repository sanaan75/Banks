using Entities;
using Entities.Journals;
using Entities.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using UseCases.Interfaces;
using UseCases.Journals;
using Web.Models;

namespace Banks.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JournalController : ControllerBase
{
    private readonly IFindJournal _findJournal;
    private readonly IDb _db;

    public JournalController(IFindJournal findJournal, IDb db)
    {
        _findJournal = findJournal;
        _db = db;
    }

    [Route("GetRecords")]
    [HttpGet]
    public JsonResult GetRecords(int year, string title, string issn, string category)
    {
        var recordsList = _findJournal.Respond(new IFindJournal.Request
        {
            Year = year,
            Title = title,
            Issn = issn,
            Category = category
        });

        return new JsonResult(recordsList);
    }

    [Route("FindByTitle")]
    [HttpGet]
    public IActionResult FindByTitle(string title, int year)
    {
        try
        {
            var items = _db.Query<JournalRecord>().Where(i => i.Year < year).FilterByJournalTitle(title);
            if (items.Any())
            {
                items = items.OrderByDescending(i => i.Year).ThenBy(i => i.QRank);
                var result = items.Select(i => new LastRecordModel
                {
                    Category = i.Category,
                    IF = i.If as double?,
                    AIF = i.Aif as double?,
                    MIF = i.Mif as double?,
                    Index = i.Index.GetCaption(),
                    Issn = i.Journal.Issn,
                    Publisher = i.Journal.Publisher,
                    EIssn = i.Journal.EIssn,
                    Year = i.Year,
                    QRank = i.QRank,
                    JournalType = i.Type!.GetCaption()
                }).FirstOrDefault();
                return Ok(result);
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [Route("GetFullInfo")]
    [HttpGet]
    public IActionResult GetFullInfo(string title, int year)
    {
        try
        {
            StringValues headerValue;
            Request.Headers.TryGetValue("JiroToken", out headerValue);
            var headerValueResult = headerValue.FirstOrDefault();

            if (headerValueResult.Equals(AppSetting.Api_Key) == false)
                return Unauthorized();

            var items = _db.Query<JournalRecord>().FilterByJournalTitle(title).FilterByYear(year);
            if (items.Any())
            {
                items = items.OrderByDescending(i => i.Year).ThenBy(i => i.QRank);
                var result = items.Select(i => new JournalRecordList
                {
                    Category = i.Category.Trim(),
                    If = i.If,
                    Aif = i.Aif,
                    Mif = i.Mif,
                    Index = i.Index,
                    IscClass = i.IscClass,
                    Year = i.Year,
                    QRank = i.QRank,
                    Type = i.Type,
                    Value = i.Value
                }).ToList();

                return Ok(result);
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Route("GetISCList")]
    [HttpGet]
    public IActionResult GetISCList(int year)
    {
        try
        {
            StringValues headerValue;
            Request.Headers.TryGetValue("JiroToken", out headerValue);
            var headerValueResult = headerValue.FirstOrDefault();

            if (headerValueResult.Equals(AppSetting.Api_Key) == false)
                return Unauthorized();

            var items = _db.Query<JournalRecord>().FilterByIndex(JournalIndex.ISC).FilterByYear(year);
            if (items.Any())
            {
                items = items.OrderByDescending(i => i.Year).ThenBy(i => i.QRank);
                var result = items.Select(i => new ISCRecordList
                {
                    Title = i.Journal.Title,
                    Category = i.Category.Trim(),
                    If = i.If,
                    QRank = i.QRank,
                    Type = i.Type,
                    ISSN = i.Journal.Issn
                }).ToList();

                return Ok(result);
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}