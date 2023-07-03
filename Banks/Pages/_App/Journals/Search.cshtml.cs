using System.ComponentModel.DataAnnotations;
using Entities.Journals;
using Entities.Utilities;
using Microsoft.AspNetCore.Mvc;
using UseCases.Interfaces;
using Web.Models;
using Web.RazorPages;

namespace Banks.Pages._App.Journals;

public class Search : AppPageModel
{
    private readonly IDb _db;

    public Search(IDb db)
    {
        _db = db;
        Indexes = EnumExt.GetList<JournalIndex>();
        q = EnumExt.GetList<JournalQRank>();
    }

    public Dictionary<string, int> Indexes { get; set; }
    public Dictionary<string, int> q { get; set; }
    public FilterModel FilterModel { get; set; }
    public List<DataItem> JournalList { get; set; }

    public void OnGet()
    {
        JournalList = new List<DataItem>();
    }


    public IActionResult OnPost(FilterModel filterModel)
    {
        var items = _db.Query<Journal>().FilterByKeyword(filterModel.Title)
            .FilterByYear(filterModel.Year)
            .FilterByIndex(filterModel.Index)
            .FilterByQRank(filterModel.q);

        if (filterModel.MinIf != null)
            items = items.Where(i => i.Records.Any(j => j.If.Value >= filterModel.MinIf));
        if (filterModel.MaxIf != null)
            items = items.Where(i => i.Records.Any(j => j.If.Value <= filterModel.MaxIf));

        JournalList = items.Select(i => new DataItem
        {
            Id = i.Id,
            Title = i.Title,
            Website = i.WebSite,
            Publisher = i.Publisher,
            Country = i.Country
        }).ToList();

        return Page();
    }
}

public class FilterModel
{
    [Display(Name = "کلید واژه")] public string Title { get; set; }

    [Display(Name = "سال")] public int? Year { get; set; }
    [Display(Name = "نمایه")] public JournalIndex? Index { get; set; }
    [Display(Name = "رتبه")] public JournalQRank? q { get; set; }
    [Display(Name = " بیشینه IF")] public decimal? MaxIf { get; set; }
    [Display(Name = "کمینه IF")] public decimal? MinIf { get; set; }
}