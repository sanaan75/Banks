using Entities.Journals;
using Entities.Utilities;
using UseCases.Interfaces;
using Web.Models;
using Web.RazorPages;

namespace Banks.Pages._App.Journals.Records;

public class Index : AppPageModel
{
    private readonly IDb _unitOfWork;

    public Index(IDb unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public List<DataModel> JournalRecords { get; set; }
    public RecordListModel AddJournalInfo { get; set; }

    public void OnGet(int id)
    {
        var journal = _unitOfWork.Query<Journal>().GetById(id);
        AddJournalInfo = new RecordListModel
        {
            Id = journal.Id,
            Title = journal.Title,
            Website = journal.WebSite,
            Publisher = journal.Publisher,
            Country = journal.Country
        };

        JournalRecords = _unitOfWork.Query<JournalRecord>().Where(i => i.JournalId == id).Select(i => new DataModel
        {
            Id = i.Id,
            JournalTitle = i.Journal.Title,
            Category = i.Category,
            Year = i.Year,
            Index = i.Index.GetCaption(),
            Type = i.Type != null ? i.Type.GetCaption() : string.Empty,
            QRank = i.QRank != null ? i.QRank.GetCaption() : string.Empty,
            If = i.If,
            Mif = i.Mif,
            Aif = i.Aif
        }).ToList();
    }
}