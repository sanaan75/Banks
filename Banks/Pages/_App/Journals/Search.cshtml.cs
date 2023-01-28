using Framework;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Entities;
using Entities.Journals;
using Web.RazorPages;

namespace JournalBank.Pages._App.Journals
{
    public class Search : AppPageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public Dictionary<string, int> Indexes { get; set; }
        public Dictionary<string, int> q { get; set; }
        public FilterModel FilterModel { get; set; }
        public List<DataItem> JournalList { get; set; }

        public Search(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Indexes = EnumExt.GetList<JournalIndex>();
            q = EnumExt.GetList<JournalQRank>();
        }

        public void OnGet()
        {
            JournalList = new List<DataItem>();
        }


        public IActionResult OnPost(FilterModel filterModel)
        {
            var items = _unitOfWork.Journals.GetAll().FilterByKeyword(filterModel.Title)
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