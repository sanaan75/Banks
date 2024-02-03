using System.Net.Http.Headers;
using Entities.Journals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UseCases.Interfaces;
using UseCases.Journals;
using Web.Models;

namespace Banks.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class ArticleController : ApplicationController
{
    private readonly IDb _db;
    private readonly IGetBestJournalInfo _getBestJournalInfo;

    private static HttpClient client = new HttpClient();

    public ArticleController(IDb db, IGetBestJournalInfo getBestJournalInfo)
    {
        _db = db;
        _getBestJournalInfo = getBestJournalInfo;
    }

    [Route("GetByDoi")]
    [HttpGet]
    public IActionResult GetByDoi(string doi)
    {
        client.BaseAddress = new Uri("https://api.crossref.org/works/" + doi);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = client.GetAsync("").Result;

        if (response.IsSuccessStatusCode == false)
            return NotFound();

        var str = response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<dynamic>(str.Result);

        var model = new ArticleModel();

        if (data["message"]["title"] != null)
            model.Title = data["message"]["title"][0];

        model.Volume = data["message"]["volume"];
        model.URL = data["message"]["URL"];
        model.Page = data["message"]["page"];
        model.Publisher = data["message"]["publisher"];
        model.Language = data["message"]["language"];

        if (data["message"]["published"]["date-parts"][0] != null)
        {
            var dateList = new List<int>();
            foreach (int item in data["message"]["published"]["date-parts"][0])
                dateList.Add(item);

            model.DateIndexed = new DateTime(dateList[0], dateList[1], dateList[2]);
        }

        model.DateCreated = data["message"]["created"]["date-time"];

        if (data["message"]["container-title"] != null)
            model.JournalTitle = data["message"]["container-title"][0];

        if (data["message"]["ISSN"] != null)
            model.ISSN = data["message"]["ISSN"][0];

        model.Issue = data["message"]["issue"];
        var member = data["message"]["member"];

        if (data["message"]["author"] != null)
            foreach (var item in data["message"]["author"])
            {
                string author = item["given"] + " " + item["family"];
                model.Authors.Add(author);
            }

        if (data["message"]["subject"] != null)
            foreach (var item in data["message"]["subject"])
                model.Categories.Add(item.ToString());

        try
        {
            client.BaseAddress = new Uri("https://api.crossref.org/members/" + member);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response2 = client.GetAsync("").Result;
            if (response2.IsSuccessStatusCode)
            {
                var str2 = response2.Content.ReadAsStringAsync();
                var data2 = JsonConvert.DeserializeObject<dynamic>(str2.Result);
                model.Location = data2["message"]["location"];
            }
        }
        catch
        {
            // ignored
        }

        if (model.DateIndexed is not null)
        {
            var query = _db.Query<JournalRecord>()
                .Where(i => i.Year < model.DateIndexed.Value.Year);

            if (string.IsNullOrWhiteSpace(model.ISSN) == false)
            {
                var record = query.FilterByJournalISSN(model.ISSN)
                    .OrderByDescending(i => i.Year)
                    .ThenBy(i => i.QRank).ThenBy(i => i.If)
                    .Select(i => new { i.If, i.Index, i.Mif, i.QRank })
                    .FirstOrDefault();

                if (record != null)
                {
                    model.IF = record.If;
                    model.Index = record.Index;
                    model.Mif = record.Mif;
                    model.QRank = record.QRank;
                }
            }

            if (string.IsNullOrEmpty(model.JournalTitle) == false)
                query = query.FilterByJournalTitle(model.JournalTitle);

            if (string.IsNullOrWhiteSpace(model.ISSN) == false)
                query = query.FilterByIssn(model.ISSN);

            model.BestInfo = _getBestJournalInfo.Respond(query);
        }

        return new JsonResult(model);
    }

    [Route("GetBestInfo")]
    [HttpGet]
    public JsonResult GetBestInfo(int year, string? journalTitle, string? issn)
    {
        var items = _db.Query<JournalRecord>().Where(i => i.Year < year);
        if (string.IsNullOrEmpty(journalTitle) == false)
            items = items.Where(i => i.Journal.Title.Trim().ToLower().Equals(journalTitle.Trim().ToLower()));
        else if (string.IsNullOrEmpty(issn) == false)
            items = items.Where(i => i.Journal.Issn.Trim().ToLower().Equals(issn.Trim().ToLower()));
        else
        {
            return new JsonResult("journal not found");
        }

        return new JsonResult(_getBestJournalInfo.Respond(items));
    }


    [Route("GetArticleInfo")]
    [HttpGet]
    public async Task<IActionResult> GetArticleInfo(string doi)
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync("https://api.crossref.org/works/" + doi);

            List<string> subjects = new();
            Dictionary<string, string> issns = new();
            List<Author> autors = new();

            if (response.IsSuccessStatusCode)
            {
                string jsonContent2 = await response.Content.ReadAsStringAsync();
                JObject jsonResponse2 = JObject.Parse(jsonContent2);
                JToken data = jsonResponse2["message"];

                foreach (var subject in data["subject"])
                {
                    subjects.Add(subject.ToString());
                }

                foreach (var issn in data["issn-type"])
                {
                    issns.Add(issn["type"].ToString(), issn["value"].ToString());
                }

                foreach (var author in data["author"])
                {
                    autors.Add(new Author
                    {
                        given = author["given"].ToString(),
                        family = author["family"].ToString(),
                        sequence = author["sequence"].ToString(),
                    });
                }

                var model = new ArticleInfoModel
                {
                    Abstract = data["abstract"] != null ? data["abstract"].ToString() : string.Empty,
                    Publisher = data["publisher"] != null ? data["publisher"].ToString() : string.Empty,
                    Indexed = data["indexed"]["date-time"] != null ? (DateTime)data["indexed"]["date-time"] : null,
                    CreatedDate = data["created"]["date-time"] != null ? (DateTime)data["created"]["date-time"] : null,
                    Page = data["page"] != null ? data["page"].ToString() : string.Empty,
                    Source = data["source"] != null ? data["source"].ToString() : string.Empty,
                    Title = data["title"] != null ? data["title"][0].ToString() : string.Empty,
                    Volume = data["volume"] != null ? data["volume"].ToString() : string.Empty,
                    Authors = autors,
                    URL = data["URL"] != null ? data["URL"].ToString() : string.Empty,
                    Language = data["language"] != null ? data["language"].ToString() : string.Empty,
                    Subjects = subjects,
                    References = data["references-count"] != null ? (int)data["references-count"] : null,
                };
                return new JsonResult(model);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }

            return new JsonResult(new ArticleInfoModel());
        }
        catch (Exception ex)
        {
            //ignored
        }

        return new JsonResult(new ArticleInfoModel());
    }

    public class ArticleInfoModel
    {
        public string Abstract { get; set; }
        public DateTime? Indexed { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Publisher { get; set; }
        public string Issue { get; set; }
        public string Page { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
        public string Volume { get; set; }
        public List<Author> Authors { get; set; }
        public List<string> Journal { get; set; }
        public string Language { get; set; }
        public string URL { get; set; }
        public List<IssnType> ISSns { get; set; }
        public List<string> Subjects { get; set; }
        public int? References { get; set; }
    }

    public class IssnType
    {
        public string value { get; set; }
        public string type { get; set; }
    }

    public class Author
    {
        public string given { get; set; }
        public string family { get; set; }
        public string sequence { get; set; }
    }
}