using Entities;
using Entities.Journals;
using Framework;
using JournalBank.Pages._App.Journals;
using Microsoft.AspNetCore.Mvc;
using Web.Models.Journals;
using Web.RazorPages;

namespace Banks.Pages._App.Journals
{
    public class Index : AppPageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public Dictionary<string, int> Indexes { get; set; }
        public FilterModel SearchModel { get; set; }
        public List<AddJournalModel> JournalList { get; set; }

        public Index(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Indexes = EnumExt.GetList<JournalIndex>();
        }

        public void OnGet()
        {
            JournalList = new List<AddJournalModel>();
        }

        public IActionResult OnPost(FilterModel searchModel)
        {
            JournalList = _unitOfWork.Journals.GetAll().FilterByKeyword(searchModel.Title)
                .FilterByYear(searchModel.Year)
                .FilterByIndex(searchModel.Index)
                .Select(i => new AddJournalModel
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
}