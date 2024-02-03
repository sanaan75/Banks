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
public class Vezaratain : AppPageModel
{
    private readonly IAddJournal _addJournal;
    private readonly IAddJournalRecord _addJournalRecord;
    private readonly IDb _db;
    private IExcelFileReader _excelFileReader;

    public Vezaratain(IDb db, IExcelFileReader excelFileReader, IAddJournalRecord addJournalRecord,
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
                    var items = dataSet.SetToList<VezaratainModel>();

                    foreach (var item in items)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(item.Title.Trim()))
                                continue;

                            var dup = _db.Query<Journal>().Any(i =>
                                i.Title.ToLower().Trim().Equals(item.Title.ToLower().Trim()));

                            if (dup == true)
                                continue;

                            var journal = _addJournal.Responce(new IAddJournal.Request
                            {
                                Title = item.Title.Trim(),
                                Issn = item.ISSN != null ? item.ISSN.Replace("-", "").Trim() : string.Empty,
                                EIssn = item.EISSN != null ? item.EISSN.Replace("-", "").Trim() : string.Empty,
                                Country = "ایران",
                                Publisher = item.Publisher.Trim()
                            });

                            _db.Save();
                            if (string.IsNullOrEmpty(item.Y1396) == false)
                            {
                                SaveRecord(journal.Id, item.Category, 1396, GetValue(item.Y1396));
                            }

                            if (string.IsNullOrEmpty(item.Y1398) == false)
                            {
                                SaveRecord(journal.Id, item.Category, 1398, GetValue(item.Y1398));
                            }

                            if (string.IsNullOrEmpty(item.Y1399) == false)
                            {
                                SaveRecord(journal.Id, item.Category, 1399, GetValue(item.Y1399));
                            }

                            if (string.IsNullOrEmpty(item.Y1400) == false)
                            {
                                SaveRecord(journal.Id, item.Category, 1400, GetValue(item.Y1400));
                            }

                            if (string.IsNullOrEmpty(item.Y1401) == false)
                            {
                                SaveRecord(journal.Id, item.Category, 1401, GetValue(item.Y1401));
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

    private JournalValue? GetValue(string? value)
    {
        switch (value)
        {
            case "الف":
                return JournalValue.A;
            case "ب":
                return JournalValue.B;
            case "ج":
                return JournalValue.C;
            case "د":
                return JournalValue.D;
            case "بین المللی":
                return JournalValue.International;
            default:
                return null;
        }
    }

    private void SaveRecord(int journalId, string category, int year, JournalValue? value)
    {
        var dup = _db.Query<JournalRecord>()
            .Where(i => i.JournalId == journalId)
            .Where(i => i.Category.ToLower().Trim().Equals(category.ToLower().Trim()))
            .Any(i => i.Year == year);

        if (dup == true)
            return;

        _db.Set<JournalRecord>().Add(new JournalRecord
        {
            JournalId = journalId,
            Category = category.Trim(),
            Index = JournalIndex.Vezaratin,
            Type = JournalType.ElmiPazhuheshi,
            Year = year,
            Value = value
        });
    }
}

public class VezaratainModel
{
    public string Title { get; set; }
    public string Category { get; set; }
    public string Y1396 { get; set; }
    public string Y1398 { get; set; }
    public string Y1399 { get; set; }
    public string Y1400 { get; set; }
    public string Y1401 { get; set; }
    public string ISSN { get; set; }
    public string EISSN { get; set; }
    public string Publisher { get; set; }
}