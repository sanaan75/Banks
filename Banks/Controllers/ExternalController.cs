using System.Net.Http.Headers;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;

namespace Banks.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExternalController : ApplicationController
{
    HtmlWeb web = new HtmlWeb();

    [Route("GetDocs")]
    [HttpGet]
    public JsonResult GetDocs(string scholarId)
    {
        var Result = new List<ScholarModel>();
        IEnumerable<HtmlNode> allNodes = new List<HtmlNode>();
        var startIndexes = new[] { 0, 100, 200, 300, 400, 500, 600, 700, 800, 900 };

        foreach (var index in startIndexes)
        {
            var url = $"https://scholar.google.com/citations?user={scholarId}&hl=en&cstart={index}&pagesize=100";
            var doc = web.Load(url);
            var nodes = doc.DocumentNode.SelectNodes("//tr[contains(@class,'gsc_a_tr')]");
            allNodes = allNodes.Union(nodes.Where(i =>
                i.InnerText.Equals("There are no articles in this profile.") == false));
        }

        foreach (HtmlNode node in allNodes)
        {
            var info = node.FirstChild.ChildNodes;
            Result.Add(new ScholarModel
            {
                Title = info.First().InnerText,
                Authors = info[1] != null ? info[1].InnerText : String.Empty,
                Journal = info[2] != null ? info[2].InnerText : String.Empty,
            });
        }

        return new JsonResult(Result);
    }

    [Route("ReadFromSJR")]
    [HttpGet]
    public JsonResult ReadFromSJR(string issn)
    {
        var Result = new List<ScholarModel>();
        IEnumerable<HtmlNode> allNodes = new List<HtmlNode>();

        var url = "https://www.scimagojr.com/journalsearch.php?q=" + issn;
        var doc = web.Load(url);

        HtmlNode aNode = doc.DocumentNode.SelectSingleNode("//a[span[@class='jrnlname']]");

        if (aNode == null)
            return new JsonResult(Result);

        string link = aNode.GetAttributeValue("href", "");
        var journal_doc = web.Load(link);
        HtmlNodeCollection trNodes = journal_doc.DocumentNode.SelectNodes("//div[@class='cellslide']/table/tbody/tr");

        if (trNodes != null && trNodes.Any())
        {
            foreach (HtmlNode trNode in trNodes)
            {
                HtmlNodeCollection tdNodes = trNode.SelectNodes("td");

                if (tdNodes != null)
                {
                    string category = tdNodes[0].InnerText.Trim();
                    string year = tdNodes[1].InnerText.Trim();
                    string quartile = tdNodes[2].InnerText.Trim();
                }
            }
        }
        
        return new JsonResult(Result);
    }

    [Route("Conferences")]
    [HttpGet]
    public JsonResult GetConferences()
    {
        var client = new HttpClient();
        client.BaseAddress =
            new Uri(
                "https://www.conferencelists.org/em-ajax/get_listings/lang=&search_datetimes%5B%5D=&search_event_types%5B%5D=&per_page=10&orderby=event_start_date&order=ASC&page=1");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = client.GetAsync("").Result;

        if (response.IsSuccessStatusCode)
        {
            var result = response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(result.Result);
            var doc = new HtmlDocument();
            var html = data.html.ToString()
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\t", string.Empty);

            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes("//*[contains(@class,'wpem-event-infomation')]");
            foreach (HtmlNode node in nodes)
            {
                var date = node.SelectNodes("//*[contains(@class,'wpem-event-infomation')]");
                var title = node.FirstChild.ChildNodes;
                var location = node.FirstChild.ChildNodes;
            }
        }

        var Result = new List<ScholarModel>();
        return new JsonResult(Result);
    }

    [Route("GetScopusDocs")]
    [HttpGet]
    public async Task<JsonResult> GetScopusDocs(string scopusId)
    {
        var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post,
            "http://eznl-https-www-scopus-com.c93ebbd0e6.di-iranpaper.ir/api/documents/search");
        request.Headers.Add("Cookie",
            "laravel_session=NjzUZI7X6OLYEuYe56eHk4RhFZCziy2KsmnJTat1; at_check=true; AMCVS_4D6368F454EC41940A4C98A6%40AdobeOrg=1; AMCV_4D6368F454EC41940A4C98A6%40AdobeOrg=-2121179033%7CMCIDTS%7C19558%7CMCMID%7C27375229094893948543614896712865407441%7CMCAAMLH-1690432376%7C6%7CMCAAMB-1690432376%7CRKhpRz8krg2tLO6pguXWp5olkAcUniQYPHaMWWgdJ3xzPWQmdj0y%7CMCOPTOUT-1689834776s%7CNONE%7CMCAID%7CNONE%7CMCCIDH%7C-800833120%7CvVersion%7C5.3.0; s_sess=%20s_cpc%3D0%3B%20s_sq%3D%3B%20s_ppvl%3Dsc%25253Ahome%25253Ahome%252C100%252C100%252C2226%252C696%252C682%252C696%252C681%252C2%252CP%3B%20e41%3D1%3B%20s_cc%3Dtrue%3B%20s_ppv%3Dsc%25253Ahome%25253Ahome%252C100%252C100%252C3479%252C696%252C682%252C696%252C681%252C2%252CL%3B; mbox=PC#137b86d595b24186acb9d35c89e88ff4.37_0#1753072833|session#d046ebeff0f643d7848f47e0e3c07c98#1689829893; s_pers=%20c19%3Dsc%253Ahome%253Ahome%7C1689829829046%3B%20v68%3D1689828027476%7C1689829829057%3B%20v8%3D1689828042788%7C1784436042788%3B%20v8_s%3DLess%2520than%25201%2520day%7C1689829842788%3B; laravel_session=NjzUZI7X6OLYEuYe56eHk4RhFZCziy2KsmnJTat1");
        request.Headers.Add("Host", "eznl-https-www-scopus-com.19105974ca.di-iranpaper.ir");
        request.Headers.Add("Origin", "http://eznl-https-www-scopus-com.19105974ca.di-iranpaper.ir");
        var content = new StringContent(
            "{\"allauthors\": true ,\"authorid\": \"{{scopusId}}\" ,\"documentClassificationEnum\": \"primary\",\"itemcount\": 200,\"offset\": 0,\"outwardlinks\":true,\"preprint\": \"\",\"query\": \"\",\"showAbstract\": true,\"sort\": \"plf-t\"}"
                .Replace("{{scopusId}}", scopusId), null, "application/json");

        request.Content = content;
        var response = await client.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();

        dynamic data = JsonConvert.DeserializeObject(result)!;
        var objects = data.items;
        var items = new List<ScopusModel>();
        foreach (var obj in objects)
        {
            try
            {
                var item = new ScopusModel();
                item.Title = obj.title.ToString();
                item.DOI = obj.doi.ToString();
                item.DocumentType = obj.documentType.ToString();
                item.PubYear = obj.pubYear;
                item.Abstract = obj.abstractText[0].ToString();
                item.ScopusId = obj.scopusId.ToString();

                var authorsNo = (Int32)obj.totalAuthors;
                var authors = new List<AuthorModel>();
                for (int i = 0; i < authorsNo; i++)
                {
                    var authorId = obj.authors[i].authorId;
                    var name = obj.authors[i].preferredName.full;
                    authors.Add(new AuthorModel
                    {
                        authorId = authorId,
                        Name = name
                    });
                }

                item.Authors = authors;
                var source = JsonConvert.DeserializeObject(obj.source.ToString());
                var sourceModel = new SourceModel();
                sourceModel.Title = source.title;
                sourceModel.ISSN = source.issn;
                sourceModel.EISSN = source.eiisn;
                sourceModel.Publisher = source.publisher;
                sourceModel.ISBN = source.isbn;
                item.Source = sourceModel;
                var sourceRelationship = JsonConvert.DeserializeObject(obj.sourceRelationship.ToString());
                item.Issue = sourceRelationship.issue;
                item.Volume = sourceRelationship.volume;
                item.Pages = sourceRelationship.pages.pageFirst + "-" + obj.sourceRelationship.pages.pageLast;
                item.PageCount = sourceRelationship.pageCount;

                items.Add(item);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        return new JsonResult(items);
    }

    public class ScopusModel
    {
        public string Title { get; set; }
        public string DOI { get; set; }
        public List<AuthorModel> Authors { get; set; }
        public string Abstract { get; set; }
        public int? PubYear { get; set; }
        public string DocumentType { get; set; }
        public string PageCount { get; set; }
        public string Volume { get; set; }
        public string Issue { get; set; }
        public string Pages { get; set; }
        public string ScopusId { get; set; }
        public SourceModel Source { get; set; }
    }

    public class AuthorModel
    {
        public string Name { get; set; }
        public string authorId { get; set; }
    }

    public class SourceModel
    {
        public string Publisher { get; set; }
        public string ISSN { get; set; }
        public string EISSN { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
    }
}