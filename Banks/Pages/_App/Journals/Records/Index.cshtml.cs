using Entities;
using Framework;
using Web.RazorPages;

namespace Banks.Pages._App.Journals.Records
{
    public class Index : AppPageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public List<DataModel> JournalRecords { get; set; }
        public RecordListModel AddJournalInfo { get; set; }

        public Index(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void OnGet(int id)
        {
            var journal = _unitOfWork.Journals.GetById(id);
            AddJournalInfo = new RecordListModel
            {
                Id = journal.Id,
                Title = journal.Title,
                Website = journal.WebSite,
                Publisher = journal.Publisher,
                Country = journal.Country
            };

            JournalRecords = _unitOfWork.JournalRecords.Find(i => i.JournalId == id).Select(i => new DataModel
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

    public class RecordListModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISSN { get; set; }
        public string Website { get; set; }
        public string Publisher { get; set; }
        public string Country { get; set; }
    }

    public class DataModel
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