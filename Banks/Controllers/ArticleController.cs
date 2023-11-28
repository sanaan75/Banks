using System.Net.Http.Headers;
using Entities.Journals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

    public ArticleController(IDb db, IGetBestJournalInfo getBestJournalInfo)
    {
        _db = db;
        _getBestJournalInfo = getBestJournalInfo;
    }

    [Route("GetByDoi")]
    [HttpGet]
    public IActionResult GetByDoi(string doi)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri("https://api.crossref.org/works/" + doi);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = client.GetAsync("").Result;

        if (response.IsSuccessStatusCode)
        {
            var str = response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(str.Result);

            var journalTitle = string.Empty;
            var issn = string.Empty;
            var title = string.Empty;
            var dateIndexed = new DateTime();
            var dateList = new List<int>();
            var authors = new List<string>();
            var categories = new List<string>();
            var location = string.Empty;
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
            var year = 0;

            if (data["message"]["published"]["date-parts"][0] != null)
            {
                foreach (int item in data["message"]["published"]["date-parts"][0]) dateList.Add(item);

                year = dateList[0];
                var month = 1;
                var day = dateList.Count();

                if (dateList.Count() > 1)
                    month = dateList[1];

                if (dateList.Count() == 3)
                    day = dateList[2];

                dateIndexed = new DateTime(year, month, day);
            }

            var dateCreated = data["message"]["created"]["date-time"];

            if (data["message"]["container-title"] != null)
                journalTitle = data["message"]["container-title"][0];

            if (data["message"]["ISSN"] != null)
                issn = data["message"]["ISSN"][0];

            var issue = data["message"]["issue"];
            var member = data["message"]["member"];

            if (data["message"]["author"] != null)
                foreach (var item in data["message"]["author"])
                {
                    string author = item["given"] + " " + item["family"];
                    authors.Add(author);
                }

            if (data["message"]["subject"] != null)
                foreach (var item in data["message"]["subject"])
                    categories.Add(item.ToString());

            try
            {
                var client2 = new HttpClient();
                client2.BaseAddress = new Uri("https://api.crossref.org/members/" + member);
                client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response2 = client2.GetAsync("").Result;
                if (response2.IsSuccessStatusCode)
                {
                    var str2 = response2.Content.ReadAsStringAsync();
                    var data2 = JsonConvert.DeserializeObject<dynamic>(str2.Result);
                    location = data2["message"]["location"];
                }
            }
            catch
            {
                // ignored
            }

            if (dateIndexed != null && string.IsNullOrEmpty(journalTitle) == false)
            {
                var journal = _db.Query<Journal>()
                    .FilterByIssn(issn)
                    .Select(i => new { i.Id })
                    .FirstOrDefault();

                if (journal != null)
                {
                    var record = _db.Query<JournalRecord>()
                        .FilterByJournal(journal.Id)
                        .Where(i => i.Year < dateIndexed.Year)
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
                else
                {
                    journal = _db.Query<Journal>()
                        .FilterByTitle(journalTitle)
                        .Select(i => new { i.Id })
                        .FirstOrDefault();

                    if (journal != null)
                    {
                        var record = _db.Query<JournalRecord>()
                            .FilterByJournal(journal.Id)
                            .Where(i => i.Year < dateIndexed.Year)
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

            var items = _db.Query<JournalRecord>().Where(i => i.Year < year);
            if (string.IsNullOrEmpty(journalTitle) == false)
                items = items.Where(i => i.Journal.Title.Trim().ToLower().Equals(journalTitle.Trim().ToLower()));
            else if (string.IsNullOrEmpty(issn) == false)
                items = items.Where(i => i.Journal.Issn.Trim().ToLower().Equals(issn.Trim().ToLower()));


            var result = new ArticleModel
            {
                Title = title,
                URL = url,
                Volume = volume,
                Authors = authors,
                DateIndexed = dateIndexed,
                DateCreated = dateCreated,
                Language = language,
                Page = page,
                Publisher = publisher,
                JournalTitle = journalTitle,
                ISSN = issn,
                Issue = issue,
                Location = location,
                Categories = categories,
                IF = IF,
                Index = Index,
                Mif = Mif,
                QRank = qRank,
                BestInfo = _getBestJournalInfo.Respond(items)
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
}