using System.Collections.Generic;
using System.Linq;
using Entities;
using Framework;
using Web.Models.Journals;
using Web.RazorPages;

namespace JournalBank.Pages._App.Users
{
    public class Index : AppPageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public List<RecordModel> JournalRecords { get; set; }
        public JournalModel JournalInfo { get; set; }

        public Index(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void OnGet(int id)
        {
            var journal = _unitOfWork.Journals.GetById(id);
            JournalInfo = new JournalModel
            {
                Id = journal.Id,
                Title = journal.Title,
                Website = journal.WebSite,
                Publisher = journal.Publisher,
                Country = journal.Country
            };

            JournalRecords = _unitOfWork.JournalRecords.Find(i => i.JournalId == id).Select(i => new RecordModel
            {
                Id = i.Id,
                JournalTitle = i.Journal.Title,
                Category = i.Category,
                Year = i.Year,
                Index = i.Index.GetCaption(),
                Type = i.Type.GetCaption(),
                QRank = i.QRank.GetCaption(),
                If = i.If,
                Mif = i.Mif,
                Aif = i.Aif
            }).ToList();
        }
    }

    public class RecordModel
    {
        public int Id { get; set; }
        public string JournalTitle { get; set; }
        public string Category { get; set; }
        public int Year { get; set; }
        public string Index { get; set; }
        public string Type { get; set; }
        public string QRank { get; set; }
        public decimal? If { get; set; }
        public decimal? Mif { get; set; }
        public decimal? Aif { get; set; }
    }
}
