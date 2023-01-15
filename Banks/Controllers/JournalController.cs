using Entities;
using Entities.Journals;
using Framework;
using Microsoft.AspNetCore.Mvc;
using UseCases.Journals;
using Web.Models.Articles;
using Web.Models.Journals;

namespace JournalBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JournalController : ControllerBase
    {
        private readonly IFindJournal _findJournal;
        private readonly IUnitOfWork _unitOfWork;

        public JournalController(IFindJournal findJournal, IUnitOfWork unitOfWork)
        {
            _findJournal = findJournal;
            _unitOfWork = unitOfWork;
        }

        // get by title
        // filter by title
        // get records
        // get isc by year
        // get jcr by year

        [Route("GetUsers")]
        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _unitOfWork.Users.GetAll();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("GetRecords")]
        [HttpGet]
        public JsonResult GetRecords(RecordSearchModel searchModel)
        {
            var recordsList = _findJournal.Respond(new IFindJournal.Request
            {
                Year = searchModel.Year.Value,
                Title = searchModel.Title,
                Issn = searchModel.Issn,
                Category = searchModel.Category
            });

            return new JsonResult(recordsList);
        }

        [Route("FindByTitle")]
        [HttpGet]
        public IActionResult FindByTitle(string title, int year)
        {
            try
            {
                var items = _unitOfWork.JournalRecords.GetAll().Where(i => i.Year < year).FilterByJournalTitle(title);
                if (items.Any())
                {
                    items = items.OrderByDescending(i => i.Year).ThenBy(i => i.QRank);
                    var result = items.Select(i => new LastRecordModel
                    {
                        Category = i.Category,
                        IF = i.If as double?,
                        AIF = i.Aif as double?,
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
                var items = _unitOfWork.JournalRecords.GetAll().FilterByJournalTitle(title).FilterByYear(year);
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
                var items = _unitOfWork.JournalRecords.GetAll().FilterByIndex(JournalIndex.ISC).FilterByYear(year);
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

    public class RecordSearchModel
    {
        public int? Year { get; set; }
        public string Title { get; set; }
        public string Issn { get; set; }
        public string Category { get; set; }
    }
}