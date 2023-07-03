using Entities.Journals;
using Entities.Utilities;
using Microsoft.AspNetCore.Mvc;
using UseCases.Interfaces;
using UseCases.Journals;
using Web.RazorPages;

namespace Banks.Pages._App.Journals.Records;

public class Create : AppPageModel
{
    private readonly IAddJournalRecord _addJournalRecord;

    private readonly IDb _db;

    public Create(IDb db, IAddJournalRecord addJournalRecord)
    {
        _db = db;
        _addJournalRecord = addJournalRecord;
        Indexes = EnumExt.GetList<JournalIndex>();
        Types = EnumExt.GetList<JournalType>();
        Ranks = EnumExt.GetList<JournalQRank>();
    }

    public DataItem RecordModel { get; set; }
    public Dictionary<string, int> Indexes { get; set; }
    public Dictionary<string, int> Types { get; set; }
    public Dictionary<string, int> Ranks { get; set; }
    public int JournalId { get; set; }

    public void OnGet(int journalId)
    {
        JournalId = journalId;
    }

    public IActionResult OnPost(DataItem recordModel)
    {
        try
        {
            var journalRecord = _addJournalRecord.Respond(new IAddJournalRecord.Request
            {
                JournalId = recordModel.JournalId,
                Year = recordModel.Year,
                Category = recordModel.Category,
                Index = (JournalIndex)recordModel.Index,
                JournalType = (JournalType)recordModel.Type,
                QRank = (JournalQRank)recordModel.QRank,
                If = recordModel.IF,
                Mif = recordModel.MIF,
                Aif = recordModel.AIF
            });
            _db.Save();
            SuccessMessage = "رکرد با موفقیت اضافه شد.";
            ModelState.Clear();
            JournalId = recordModel.JournalId;
        }
        catch
        {
            ErrorMessage = "عملیات با خطا مواجه شد";
        }

        return Page();
    }
}

public class DataItem
{
    public int JournalId { get; set; }
    public string Category { get; set; }
    public int Year { get; set; }
    public int? Index { get; set; }
    public int? Type { get; set; }
    public int? QRank { get; set; }
    public decimal? IF { get; set; }
    public decimal? MIF { get; set; }
    public decimal? AIF { get; set; }
}