using Entities.Journals;
using Entities.Utilities;
using Microsoft.AspNetCore.Mvc;
using UseCases.Interfaces;
using Web.RazorPages;

namespace Banks.Pages._App.Journals;

public class Index : AppPageModel
{
    private readonly IDb _db;

    public Index(IDb db)
    {
        _db = db;
        Indexes = EnumExt.GetList<JournalIndex>();
    }

    public Dictionary<string, int> Indexes { get; set; }
    public FilterModel SearchModel { get; set; }
    public List<DataItem> JournalList { get; set; }

    public void OnGet()
    {
        JournalList = new List<DataItem>();
    }

    public IActionResult OnPost(FilterModel searchModel)
    {
        JournalList = _db.Query<Journal>().FilterByKeyword(searchModel.Title)
            .FilterByYear(searchModel.Year)
            .FilterByIndex(searchModel.Index)
            .Select(i => new DataItem
            {
                Id = i.Id,
                Title = i.Title,
                Website = i.WebSite,
                Publisher = i.Publisher,
                Country = i.Country
            }).ToList();

        return Page();
    }

    public class DataItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISSN { get; set; }
        public string Website { get; set; }
        public string Publisher { get; set; }
        public string Country { get; set; }
    }
}