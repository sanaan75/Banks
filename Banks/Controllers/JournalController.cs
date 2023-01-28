using Entities.Journals;
using Entities.Models;
using Framework;
using Microsoft.AspNetCore.Mvc;

namespace Banks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JournalController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public JournalController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("GetByTitle")]
        [HttpGet]
        public IActionResult GetByTitle(string title)
        {
            try
            {
                var items = _unitOfWork.Journals.GetAll().FilterByTitle(title);
                if (items.Any())
                {
                    var result = items.Select(i => new JournalModel
                    {
                        Title = i.Title,
                        ISSN = i.Issn,
                        Publisher = i.Publisher,
                        EISSN = i.EIssn,
                        Country = i.Country,
                        WebSite = i.WebSite
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


        [Route("GetByTitleAndYear")]
        [HttpGet]
        public IActionResult GetByTitleAndYear(string title, int year)
        {
            try
            {
                var items = _unitOfWork.Journals.GetAll().FilterByTitle(title).FilterByYear(year);
                if (items.Any())
                {
                    var result = items.Select(i => new JournalModel
                    {
                        Title = i.Title,
                        ISSN = i.Issn,
                        Publisher = i.Publisher,
                        EISSN = i.EIssn,
                        Country = i.Country,
                        WebSite = i.WebSite
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


        [Route("FilterByKey")]
        [HttpGet]
        public IActionResult FilterByKey(string key)
        {
            try
            {
                var items = _unitOfWork.Journals.GetAll().FilterByKey(key);
                if (items.Any())
                {
                    var result = items.Select(i => new JournalModel
                    {
                        Title = i.Title,
                        ISSN = i.Issn,
                        Publisher = i.Publisher,
                        EISSN = i.EIssn,
                        Country = i.Country,
                        WebSite = i.WebSite
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


        [Route("GetListByIndex")]
        [HttpGet]
        public IActionResult GetListByIndex(int year, JournalIndex index)
        {
            try
            {
                var items = _unitOfWork.JournalRecords.GetAll().FilterByIndex(index).FilterByYear(year);
                if (items.Any())
                {
                    items = items.OrderByDescending(i => i.Year).ThenBy(i => i.QRank);
                    var result = items.Select(i => new RecordsByIndexModel
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


}