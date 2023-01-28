using Entities.Journals;
using Entities.Models;
using Framework;
using Microsoft.AspNetCore.Mvc;
using UseCases.Journals;
using Web.Models.Journals;

namespace Banks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFindJournal _findJournal;

        public RecordController(IUnitOfWork unitOfWork, IFindJournal findJournal)
        {
            _unitOfWork = unitOfWork;
            _findJournal = findJournal;
        }

        [Route("GetByTitleAndYear")]
        [HttpGet]
        public IActionResult GetByTitleAndYear(string title, int year)
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

        [Route("Records")]
        [HttpGet]
        public JsonResult Records(RecordsModel searchModel)
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
    }
}