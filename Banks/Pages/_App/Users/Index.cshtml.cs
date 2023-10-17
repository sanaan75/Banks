using Entities.Journals;
using Entities.Utilities;
using UseCases.Interfaces;
using Web.Models;
using Web.RazorPages;

namespace Banks.Pages._App.Users;

public class Index : AppPageModel
{
    private readonly IDb _db;

    public Index(IDb db)
    {
        _db = db;
    }

    public List<RecordModel> JournalRecords { get; set; }
    public DataItem AddJournalInfo { get; set; }

    public void OnGet(int id)
    {
        var journal = _db.Query<Journal>().GetById(id);
        AddJournalInfo = new DataItem
        {
            Id = journal.Id,
            Title = journal.Title,
            Website = journal.WebSite,
            Publisher = journal.Publisher,
            Country = journal.Country
        };

        JournalRecords = _db.Query<JournalRecord>().Where(i => i.JournalId == id).Select(i => new RecordModel
        {
            Id = i.Id,
            JournalTitle = i.Journal.Title,
            Category = i.Category,
            Year = i.Year,
            Index = i.Index.GetCaption(),
            Type = i.Type!.GetCaption(),
            QRank = i.QRank!.GetCaption(),
            If = i.If,
            Mif = i.Mif,
            Aif = i.Aif
        }).ToList();
    }
}



public class DataItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ISSN { get; set; }
    public string? Website { get; set; }
    public string? Publisher { get; set; }
    public string? Country { get; set; }
}