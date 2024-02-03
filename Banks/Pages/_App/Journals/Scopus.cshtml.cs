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
public class Scopus : AppPageModel
{
    private readonly IAddJournal _addJournal;
    private readonly IAddJournalRecord _addJournalRecord;
    private readonly IDb _db;
    private IExcelFileReader _excelFileReader;

    public Scopus(IDb db, IExcelFileReader excelFileReader, IAddJournalRecord addJournalRecord,
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
                    var items = dataSet.SetToList<ScopusModel>();

                    foreach (var item in items)
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(item.Title))
                                continue;

                            var journal = _db.Query<Journal>().FirstOrDefault(i =>
                                i.Title.Replace(" - ", "-").ToLower().Trim().Equals(item.Title.Replace(" - ", "-").ToLower().Trim()));

                            if (journal is null)
                            {
                                journal = _addJournal.Responce(new IAddJournal.Request
                                {
                                    Title = item.Title.Trim(),
                                    Country = item.Publisher != null ? item.Country!.Trim() : string.Empty,
                                    Publisher = item.Publisher != null ? item.Publisher!.Trim() : string.Empty
                                });

                                _db.Save();
                            }


                            var _issn = Convert.ToString(item.ISSN);
                            if (_issn.Trim().Length > 0)
                            {
                                var issns = _issn.Split(",");
                                journal.Issn = issns.Length > 0 ? issns[0].Replace("-", "") : String.Empty;
                                journal.EIssn = issns.Length > 1 ? issns[1].Replace("-", "") : String.Empty;
                            }

                            var categories = item.Categories!.Split(";");

                            foreach (var category in categories)
                            {
                                if (category.Contains("(Q") == false)
                                    continue;

                                var _catergory = category.Trim().Substring(0, category.Length - 5);
                                var _rank = category.Trim().Substring(category.Length - 5).Replace("(", "")
                                    .Replace(")", "").Trim();

                                var recordDup = _db.Query<JournalRecord>()
                                    .Where(i => i.JournalId == journal.Id)
                                    .Where(i => i.Category.ToLower().Trim().Equals(category.ToLower().Trim()))
                                    .Any(i => i.Year == readModel.Year);

                                if (recordDup == true)
                                    continue;

                                _db.Set<JournalRecord>().Add(new JournalRecord
                                {
                                    Journal = journal,
                                    Category = _catergory.Trim(),
                                    Index = JournalIndex.Scopus,
                                    Type = JournalType.ElmiPazhuheshi,
                                    QRank = GetRank(_rank.ToUpper()),
                                    Year = readModel.Year,
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            // ignored
                        }
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

    private JournalQRank? GetRank(string? value)
    {
        switch (value)
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

public class ScopusModel
{
    public string? Title { get; set; }
    public object? ISSN { get; set; }
    public string? Country { get; set; }
    public string? Categories { get; set; }
    public string? Publisher { get; set; }
}