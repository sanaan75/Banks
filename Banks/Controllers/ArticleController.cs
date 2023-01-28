using System.Net.Http.Headers;
using Entities.Journals;
using Entities.Models;
using Framework;
using Microsoft.AspNetCore.Mvc;

namespace Banks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ArticleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("GetByDoi")]
        [HttpGet]
        public IActionResult GetByDoi(string doi)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.crossref.org/works/" + doi);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("").Result;

            if (response.IsSuccessStatusCode)
            {
                var str = response.Content.ReadAsStringAsync();
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(str.Result);

                var journal_title = string.Empty;
                var issn = string.Empty;
                var title = string.Empty;
                var date_indexed = new DateTime();
                DateTime date = new DateTime();
                List<int> dateList = new List<int>();
                List<string> authors = new List<string>();
                List<string> categories = new List<string>();
                string location = string.Empty;
                decimal? IF = null;
                JournalIndex? Index = null;
                decimal? Mif = null;
                JournalQRank? qRank = null;

                var volume = data["message"]["volume"];

                if (data["message"]["title"] != null)
                    title = data["message"]["title"][0];

                var url = data["message"]["URL"];
                var page = data["message"]["page"];
                var publisher = data["message"]["publisher"];
                var language = data["message"]["language"];

                if (data["message"]["published"]["date-parts"][0] != null)
                {
                    foreach (int item in data["message"]["published"]["date-parts"][0])
                    {
                        dateList.Add(item);
                    }

                    int year = dateList[0];
                    int month = 1;
                    int day = dateList.Count();

                    if (dateList.Count() > 1)
                        month = dateList[1];

                    if (dateList.Count() == 3)
                        day = dateList[2];

                    date_indexed = new DateTime(year, month, day);
                }

                var date_created = data["message"]["created"]["date-time"];

                if (data["message"]["container-title"] != null)
                    journal_title = data["message"]["container-title"][0];

                if (data["message"]["ISSN"] != null)
                    issn = data["message"]["ISSN"][0];

                var issue = data["message"]["issue"];
                var member = data["message"]["member"];

                if (data["message"]["author"] != null)
                {
                    foreach (var item in data["message"]["author"])
                    {
                        string _author = item["given"] + " " + item["family"];
                        authors.Add(_author);
                    }
                }

                if (data["message"]["subject"] != null)
                {
                    foreach (var item in data["message"]["subject"])
                    {
                        categories.Add(item.ToString());
                    }
                }

                try
                {
                    HttpClient membersClient = new HttpClient();
                    membersClient.BaseAddress = new Uri("https://api.crossref.org/members/" + member);
                    membersClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage responseForMembers = membersClient.GetAsync("").Result;

                    if (responseForMembers.IsSuccessStatusCode)
                    {
                        var str2 = responseForMembers.Content.ReadAsStringAsync();
                        var data2 = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(str2.Result);
                        location = data2["message"]["location"];
                    }

                    if (date_indexed != null && journal_title != null)
                    {
                        var journalId = _unitOfWork.Journals.GetAll()
                            .FilterByTitle(journal_title)
                            .Select(i => i.Id)
                            .FirstOrDefault();

                        if (journalId != null)
                        {
                            var record = _unitOfWork.JournalRecords.GetAll()
                                .FilterByJournal(journalId)
                                .Where(i => i.Year < date.Year)
                                .OrderByDescending(i => i.Year)
                                .ThenBy(i => i.QRank).ThenBy(i => i.If)
                                .Select(i => new { i.If, i.Index, i.Mif, i.QRank })
                                .FirstOrDefault();

                            if (record != null)
                            {
                                IF = record.If;
                                Index = record.Index;
                                Mif = record.Mif;
                                qRank = record.QRank;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500);
                }

                var result = new ArticleModel
                {
                    Title = title,
                    URL = url,
                    Volume = volume,
                    Authors = authors,
                    DateIndexed = date_indexed,
                    DateCreated = date_created,
                    Language = language,
                    Page = page,
                    Publisher = publisher,
                    JournalTitle = journal_title,
                    ISSN = issn,
                    Issue = issue,
                    Location = location,
                    Categories = categories,
                    IF = IF,
                    Index = Index,
                    Mif = Mif,
                    QRank = qRank
                };

                client.Dispose();
                return new JsonResult(result);
            }

            switch ((int)response.StatusCode)
            {
                case 401:
                    return Unauthorized();
                case 403:
                    return Forbid();
                case 404:
                    return NotFound();
                default:
                    return BadRequest();
            }
        }
    }
}