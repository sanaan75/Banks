using Entities.Journals;
using Entities.Models;
using Entities.Utilities;
using Microsoft.AspNetCore.Mvc;
using UseCases;
using UseCases.Interfaces;
using UseCases.Journals;
using Web.RazorPages;

namespace Banks.Pages._App.Journals;

[RequestFormLimits(MultipartBodyLengthLimit = 1048576000)]
public class UpdateByYear : AppPageModel
{
    private readonly IAddJournal _addJournal;
    private readonly IAddJournalRecord _addJournalRecord;
    private readonly IDb _db;
    private IExcelFileReader _excelFileReader;

    public UpdateByYear(IDb db, IExcelFileReader excelFileReader,
        IAddJournalRecord addJournalRecord,
        IAddJournal addJournal)
    {
        _db = db;
        _excelFileReader = excelFileReader;
        _addJournalRecord = addJournalRecord;
        _addJournal = addJournal;
        Indexes = EnumExt.GetList<JournalIndex>();
    }

    public Dictionary<string, int> Indexes { get; set; }
    public ReadModel ReadModel { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost(ReadModel readModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (readModel.FormFile.Length > 0)
                {
                    var dataSet = _excelFileReader.ToDataSet(readModel.FormFile);
                    var items = dataSet.SetToList<Model>();
                    var journals = _db.Query<Journal>().Select(i => new { i.Id, i.Title }).ToList();
                    try
                    {
                        foreach (var item in items)
                        {
                            if (string.IsNullOrEmpty(item.Title.Trim()))
                                continue;

                            if (item.Categories.Equals("N/A"))
                                continue;

                            var categories = Clean(item.Categories).Split(",");

                            var journal = journals.FirstOrDefault(i =>
                                i.Title.Trim().ToLower() == item.Title.Trim().ToLower());

                            if (journal != null)
                            {
                                foreach (var category in categories)
                                {
                                    //var category = cat.Trim();

                                    var records = _db.Query<JournalRecord>()
                                        .FilterByJournal(journal.Id)
                                        .FilterByYear(readModel.Year)
                                        .FilterByIndex(readModel.Index).ToList();

                                    var record = records.FirstOrDefault(k =>
                                        k.Category.Trim().ToLower() == category.Trim().ToLower());

                                    if (record != null)
                                    {
                                        record.If = item.IF;
                                        record.QRank = GetQrank(item.QRank);
                                    }
                                    else
                                    {
                                        var historyAlreadyExists = _db.Query<JournalRecord>()
                                            .Where(i => i.JournalId == journal.Id)
                                            .FilterByCategory(category)
                                            .Any(i => i.Year == readModel.Year);

                                        if (historyAlreadyExists)
                                            continue;

                                        _db.Set<JournalRecord>().Add(new JournalRecord
                                        {
                                            JournalId = journal.Id,
                                            Year = readModel.Year,
                                            Index = readModel.Index,
                                            Type = null,
                                            Value = null,
                                            IscClass = null,
                                            QRank = GetQrank(item.QRank),
                                            If = item.IF,
                                            Category = category,
                                            Mif = null,
                                            Aif = null,
                                            CreatorId = 1, //_actorService.UserId,
                                            CreateDate = DateTime.UtcNow
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var Issn = string.Empty;
                                if (item.ISSN != null && item.ISSN.Equals("N/A") == false)
                                    Issn = item.ISSN;

                                var _newJournal = _addJournal.Responce(new IAddJournal.Request
                                {
                                    Title = item.Title.Trim(),
                                    Issn = Issn,
                                    EIssn = item.EISSN
                                });

                                foreach (var category in categories)
                                {
                                    _db.Set<JournalRecord>().Add(new JournalRecord
                                    {
                                        Journal = _newJournal,
                                        Year = readModel.Year,
                                        Index = readModel.Index,
                                        Type = null,
                                        Value = null,
                                        IscClass = null,
                                        QRank = GetQrank(item.QRank),
                                        If = item.IF,
                                        Category = category,
                                        Mif = null,
                                        Aif = null,
                                        CreatorId = 1,
                                        CreateDate = DateTime.UtcNow
                                    });
                                }
                            }
                            _db.Save();
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        // ignored
                    }
                }

                SuccessMessage = "با موفقیت اپدیت شد";
            }
            catch (Exception ex)
            {
                ErrorMessage = "عملیات با خطا مواجه شد";
            }
        }
        else
        {
            ErrorMessage = "لطفا مقادیر خواسته شده را تکمیل نمایید.";
        }

        return Page();
    }

    private string Clean(string text)
    {
        return text.Replace(" - SSCI", string.Empty)
            .Replace(" - SCIE", string.Empty);
    }

    private JournalQRank? GetQrank(string rank)
    {
        switch (rank)
        {
            case "Q1":
                return JournalQRank.Q1;
            case "Q2":
                return JournalQRank.Q2;
            case "Q3":
                return JournalQRank.Q3;
            case "Q4":
                return JournalQRank.Q4;
            default:
                return null;
        }
    }
}

public class Model
{
    public string Title { get; set; }
    public string ISSN { get; set; }
    public string EISSN { get; set; }
    public string QRank { get; set; }
    public decimal? IF { get; set; }
    public string Categories { get; set; }
}