using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Entities.Journals;
using Entities.Utilities;
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

    private HttpClient client = new HttpClient();

    public ArticleController(IDb db, IGetBestJournalInfo getBestJournalInfo)
    {
        _db = db;
        _getBestJournalInfo = getBestJournalInfo;
    }

    [Route("FetchByDoi")]
    [HttpGet]
    public IActionResult FetchByDoi(string doi)
    {
        client.BaseAddress = new Uri("https://api.crossref.org/works/" + doi);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = client.GetAsync("").Result;

        if (response.IsSuccessStatusCode == false)
            return NotFound();

        var jsonString = response.Content.ReadAsStringAsync();
        Root root = JsonConvert.DeserializeObject<Root>(jsonString.Result);
        var data = root.Message;

        var model = new ArticleModel();

        if (data.Title.Any())
            model.Title = data.Title[0];

        model.Volume = data.Volume;
        model.URL = data.Url;
        model.Page = data.Page;
        model.Publisher = data.Publisher;
        model.Language = data.Language;

        if (data.Published.DateParts is not null)
        {
            var dateList = new List<int>();
            foreach (int item in data.Published.DateParts[0])
                dateList.Add(item);

            model.DateIndexed = new DateTime(dateList[0], dateList[1], dateList[2]);
        }

        if (data.Indexed.Timestamp > 0)
        {
            try
            {
                model.DateIndexed = new DateTime(1970, 1, 1, 0, 0, 0, 0)
                    .AddSeconds(Math.Round(data.Indexed.Timestamp / 1000d)).ToLocalTime();
            }
            catch (Exception e)
            {
            }
        }

        model.DateCreated = data.Created.DateTime;

        if (data.ContainerTitle.Any())
            model.JournalTitle = data.ContainerTitle[0].ToString();

        if (data.Issn is not null)
            model.ISSN = data.Issn[0].CleanIssn();

        var member = data.Member;
        List<string> authers = new();
        if (data.Author is not null)
            foreach (var item in data.Author)
            {
                string author = item.Given + " " + item.Family;
                authers.Add(author);
            }

        model.Authors = authers;

        if (data.Subject is not null)
            foreach (var item in data.Subject)
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
                var issn = model.ISSN.CleanIssn();
                var record = query.FilterByJournalISSN(issn)
                    .OrderByDescending(i => i.Year)
                    .ThenBy(i => i.QRank)
                    .ThenBy(i => i.If)
                    .Select(i => new
                    {
                        i.Index,
                        i.QRank,
                        i.If,
                        i.Mif,
                        i.Aif,
                    }).FirstOrDefault();

                if (record != null)
                {
                    model.IF = record.If;
                    model.Index = record.Index;
                    model.Mif = record.Mif;
                    model.Aif = record.Aif;
                    model.QRank = record.QRank;
                }
            }

            if (string.IsNullOrEmpty(model.JournalTitle) == false)
                query = query.FilterByJournalTitle(model.JournalTitle);

            if (string.IsNullOrWhiteSpace(model.ISSN) == false)
                query = query.FilterByIssn(model.ISSN.CleanIssn());

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
            items = items.Where(i => i.Journal.NormalizedTitle.Equals(journalTitle.VacuumString()));

        else if (string.IsNullOrEmpty(issn) == false)
            items = items.Where(i => i.Journal.Issn.Equals(issn.CleanIssn()));
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

    // ----------------------------

    public class Root
    {
        public string Status { get; set; }
        [JsonPropertyName("message-type")] public string MessageType { get; set; }
        [JsonPropertyName("message-version")] public string MessageVersion { get; set; }
        public Message Message { get; set; }
    }

    public class Message
    {
        [JsonProperty("indexed")] public Indexed Indexed { get; set; }

        [JsonProperty("publisher-location")] public string PublisherLocation { get; set; }

        [JsonProperty("reference-count")] public int ReferenceCount { get; set; }

        [JsonProperty("publisher")] public string Publisher { get; set; }

        [JsonProperty("isbn-type")] public List<IsbnType> IsbnType { get; set; }

        [JsonProperty("DOI")] public string Doi { get; set; }

        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("created")] public Created Created { get; set; }
        [JsonProperty("page")] public string Page { get; set; }

        [JsonProperty("source")] public string Source { get; set; }
        [JsonProperty("title")] public List<string> Title { get; set; }
        [JsonProperty("author")] public List<JAuthor> Author { get; set; }
        [JsonProperty("member")] public string Member { get; set; }

        [JsonProperty("container-title")] public List<string> ContainerTitle { get; set; }

        [JsonProperty("original-title")] public List<object> OriginalTitle { get; set; }

        [JsonProperty("deposited")] public Deposited Deposited { get; set; }

        [JsonProperty("subtitle")] public List<object> Subtitle { get; set; }

        [JsonProperty("issued")] public PublishedOnline Issued { get; set; }

        [JsonProperty("ISBN")] public List<string> Isbn { get; set; }

        [JsonProperty("URL")] public string Url { get; set; }

        [JsonProperty("ISSN")] public List<string> Issn { get; set; }

        [JsonProperty("issn-type")] public List<IsbnType> IssnType { get; set; }

        [JsonProperty("subject")] public List<object> Subject { get; set; }
        [JsonProperty("published")] public PublishedOnline Published { get; set; }
        public string? Volume { get; set; }
        public string? Language { get; set; }
    }

    public class Indexed
    {
        [JsonPropertyName("date-parts")] public List<List<int>> DateParts { get; set; }
        [JsonPropertyName("date-time")] public DateTime DateTime { get; set; }
        public long Timestamp { get; set; }
    }

    public class IsbnType
    {
        public string Value { get; set; }
        public string Type { get; set; }
    }

    public class ContentDomain
    {
        public List<object> Domain { get; set; }

        [JsonPropertyName("crossmark-restriction")]
        public bool CrossmarkRestriction { get; set; }
    }

    public class Created
    {
        [JsonPropertyName("date-parts")] public List<List<int>> DateParts { get; set; }
        [JsonPropertyName("date-time")] public DateTime DateTime { get; set; }
        public long Timestamp { get; set; }
    }

    public class JAuthor
    {
        public string Given { get; set; }
        public string Family { get; set; }
        public string Sequence { get; set; }
        public List<object> Affiliation { get; set; }
    }

    public class PublishedOnline
    {
        [JsonPropertyName("date-parts")] public List<List<int>> DateParts { get; set; }
    }

    public class Deposited
    {
        [JsonPropertyName("date-parts")] public List<List<int>> DateParts { get; set; }
        [JsonPropertyName("date-time")] public DateTime DateTime { get; set; }
        public long Timestamp { get; set; }
    }

    public class Resource
    {
        public Primary Primary { get; set; }
    }

    public class Primary
    {
        public string URL { get; set; }
    }

    public class Issued
    {
        [JsonPropertyName("date-parts")] public List<List<int>> DateParts { get; set; }
    }

    public class Relation
    {
    }

    public class Published
    {
        [JsonPropertyName("date-parts")] public List<List<int>> DateParts { get; set; }
    }
}