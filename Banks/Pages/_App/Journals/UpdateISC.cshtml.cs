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
public class UpdateISC : AppPageModel
{
    private readonly IAddJournal _addJournal;
    private readonly IAddJournalRecord _addJournalRecord;
    private readonly IDb _db;
    private IExcelFileReader _excelFileReader;

    public UpdateISC(IDb db, IExcelFileReader excelFileReader, IAddJournalRecord addJournalRecord,
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
            try
            {
                if (readModel.FormFile.Length > 0)
                {
                    var dataSet = _excelFileReader.ToDataSet(readModel.FormFile);
                    var items = dataSet.SetToList<ISC_Model>();
                    var journals = _db.Query<Journal>().Select(i => new { i.Id, i.Title });

                    foreach (var item in items)
                        try
                        {
                            if (string.IsNullOrEmpty(item.Title.Trim()))
                                continue;

                            // if (item.Year == null)
                            //     continue;
                            if (item.Categories.Equals("N/A"))
                                continue;

                            var categories = item.Categories.Split(",");

                            var journal = journals.FirstOrDefault(i =>
                                i.Title.Trim().ToLower() == item.Title.Trim().ToLower());

                            if (journal != null)
                            {
                                foreach (var cat in categories)
                                {
                                    var category = cat.Substring(0, cat.Length - 6).Trim();

                                    var records = _db.Query<JournalRecord>()
                                        .FilterByJournal(journal.Id)
                                        .FilterByYear(readModel.Year)
                                        .FilterByIndex(readModel.Index).ToList();

                                    var record = records.FirstOrDefault(k =>
                                        k.Category.Trim().ToLower() == category.Trim().ToLower());

                                    if (record != null)
                                    {
                                        record.If = item.IF;

                                        if (cat.Contains("("))
                                        {
                                            var startIndex = cat.IndexOf('(');
                                            var qRank = cat.Substring(startIndex).Trim();
                                            qRank = qRank.Replace("(", "").Replace(")", "").Trim();
                                            record.QRank = GetQrank(qRank);
                                        }
                                    }
                                    else
                                    {
                                        var qRank = string.Empty;
                                        if (cat.Contains("("))
                                        {
                                            var startIndex = cat.IndexOf('(');
                                            qRank = cat.Substring(startIndex).Trim();
                                            qRank = qRank.Replace("(", "").Replace(")", "").Trim();
                                        }

                                        _addJournalRecord.Respond(new IAddJournalRecord.Request
                                        {
                                            JournalId = journal.Id,
                                            Category = category,
                                            Year = readModel.Year,
                                            If = item.IF,
                                            QRank = GetQrank(qRank),
                                            Index = readModel.Index
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var Issn = string.Empty;
                                if (item.ISSN != null && item.ISSN.Equals("N/A") == false)
                                    Issn = item.ISSN;

                                var journalNew = _addJournal.Responce(new IAddJournal.Request
                                {
                                    Title = item.Title.Trim(),
                                    Issn = Issn,
                                    EIssn = item.EISSN
                                });
                                _db.Save();

                                foreach (var cat in categories)
                                {
                                    var qRank = string.Empty;
                                    var category = string.Empty;

                                    if (cat.Contains("("))
                                    {
                                        var startIndex = cat.IndexOf('(');
                                        qRank = cat.Substring(startIndex).Trim();
                                        qRank = qRank.Replace("(", "").Replace(")", "").Trim();
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    _addJournalRecord.Respond(new IAddJournalRecord.Request
                                    {
                                        JournalId = journalNew.Id,
                                        Category = category,
                                        Year = readModel.Year,
                                        If = item.IF,
                                        QRank = GetQrank(qRank),
                                        Index = readModel.Index
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }

                    _db.Save();
                }

                SuccessMessage = "با موفقیت اپدیت شد";
            }
            catch (Exception ex)
            {
                ErrorMessage = "عملیات با خطا مواجه شد";
            }
        else
            ErrorMessage = "لطفا مقادیر خواسته شده را تکمیل نمایید.";

        return Page();
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

public class ISC_Model
{
    public string Title { get; set; }
    public string ISSN { get; set; }
    public string EISSN { get; set; }

    public decimal? IF { get; set; }

    //public int? Year { get; set; }
    public string Categories { get; set; }
}