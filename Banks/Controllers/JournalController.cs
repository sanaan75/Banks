using Entities.Journals;
using Entities.Utilities;
using Microsoft.AspNetCore.Mvc;
using UseCases.Interfaces;
using UseCases.Journals;
using Web.Models;

namespace Banks.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JournalController : ApplicationController
{
    private readonly IDb _db;
    private readonly IFindJournal _findJournal;
    private readonly IIsJournalBlackList _isJournalBlackList;
    private readonly IGetBestJournalInfo _getBestJournalInfo;


    public JournalController(IFindJournal findJournal, IDb db, IIsJournalBlackList isJournalBlackList,
        IGetBestJournalInfo getBestJournalInfo)
    {
        _findJournal = findJournal;
        _db = db;
        _isJournalBlackList = isJournalBlackList;
        _getBestJournalInfo = getBestJournalInfo;
    }

    [Route("FindByTitle")]
    [HttpGet]
    public IActionResult FindByTitle(string title)
    {
        try
        {
            var vacuumedTitle = title.VacuumString();
            var items = _db.Query<Journal>().FilterByTitle(vacuumedTitle);
            return Ok(items.ToList());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Route("GetRecords")]
    [HttpGet]
    public JsonResult GetRecords(int year, string? title, string? issn, string? category)
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

    [Route("GetFullInfo")]
    [HttpGet]
    public IActionResult GetFullInfo(string title, int year)
    {
        try
        {
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

    [Route("IsBlackList")]
    [HttpGet]
    public JsonResult IsBlackList(string title)
    {
        var isBlackList = _isJournalBlackList.Responce(title);

        return new JsonResult(isBlackList);
    }
}