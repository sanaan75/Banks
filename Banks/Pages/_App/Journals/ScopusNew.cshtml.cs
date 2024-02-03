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
public class ScopusNew : AppPageModel
{
    private readonly IAddJournal _addJournal;
    private readonly IAddJournalRecord _addJournalRecord;
    private readonly IDb _db;
    private IExcelFileReader _excelFileReader;

    public ScopusNew(IDb db, IExcelFileReader excelFileReader, IAddJournalRecord addJournalRecord,
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
                    var items = dataSet.SetToList<Scopus2Model>();

                    foreach (var item in items)
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(item.Title))
                                continue;

                            var journal = _db.Query<Journal>().FirstOrDefault(i =>
                                i.Title.Replace(" - ", "-").ToLower().Trim()
                                    .Equals(item.Title.Replace(" - ", "-").ToLower().Trim()));

                            var _country = item.Country != null ? Convert.ToString(item.Country) : String.Empty;

                            if (journal is null)
                            {
                                journal = _addJournal.Responce(new IAddJournal.Request
                                {
                                    Title = item.Title.Trim(),
                                    Country = _country,
                                    Publisher = item.Publisher != null ? item.Publisher!.Trim() : string.Empty
                                });
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(_country) == false)
                                    journal.Country = _country;
                            }

                            _db.Save();

                            var _issn = Convert.ToString(item.ISSN);
                            if (_issn.Trim().Length > 0)
                            {
                                var issns = _issn.Split(",");
                                journal.Issn = issns.Length > 0 ? issns[0].Replace("-", "") : String.Empty;
                                journal.EIssn = issns.Length > 1 ? issns[1].Replace("-", "") : String.Empty;
                            }

                            var _category1 = Convert.ToString(item.Category1);
                            var _category2 = Convert.ToString(item.Category2);
                            var _category3 = Convert.ToString(item.Category3);
                            var _category4 = Convert.ToString(item.Category4);
                            var _category5 = Convert.ToString(item.Category5);

                            if (string.IsNullOrWhiteSpace(_category1) == false)
                            {
                                if (_category1.Contains("(Q") == false)
                                    continue;

                                var _catergory = _category1.Trim().Substring(0, _category1.Length - 5);
                                var _rank = _category1.Trim().Substring(_category1.Length - 5).Replace("(", "")
                                    .Replace(")", "").Trim();

                                var recordDup = _db.Query<JournalRecord>()
                                    .Where(i => i.JournalId == journal.Id)
                                    .Where(i => i.Category.ToLower().Trim().Equals(_category1.ToLower().Trim()))
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
                                    Year = readModel.Year
                                });
                            }

                            if (string.IsNullOrWhiteSpace(_category2) == false)
                            {
                                if (_category2.Contains("(Q") == false)
                                    continue;

                                var _catergory = _category2.Trim().Substring(0, _category2.Length - 5);
                                var _rank = _category2.Trim().Substring(_category2.Length - 5).Replace("(", "")
                                    .Replace(")", "").Trim();

                                var recordDup = _db.Query<JournalRecord>()
                                    .Where(i => i.JournalId == journal.Id)
                                    .Where(i => i.Category.ToLower().Trim().Equals(_category2.ToLower().Trim()))
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
                                    Year = readModel.Year
                                });
                            }

                            if (string.IsNullOrWhiteSpace(_category3) == false)
                            {
                                if (_category3.Contains("(Q") == false)
                                    continue;

                                var _catergory = _category3.Trim().Substring(0, _category3.Length - 5);
                                var _rank = _category3.Trim().Substring(_category3.Length - 5).Replace("(", "")
                                    .Replace(")", "").Trim();

                                var recordDup = _db.Query<JournalRecord>()
                                    .Where(i => i.JournalId == journal.Id)
                                    .Where(i => i.Category.ToLower().Trim().Equals(_category3.ToLower().Trim()))
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
                                    Year = readModel.Year
                                });
                            }

                            if (string.IsNullOrWhiteSpace(_category4) == false)
                            {
                                if (_category4.Contains("(Q") == false)
                                    continue;

                                var _catergory = _category4.Trim().Substring(0, _category4.Length - 5);
                                var _rank = _category4.Trim().Substring(_category4.Length - 5).Replace("(", "")
                                    .Replace(")", "").Trim();

                                var recordDup = _db.Query<JournalRecord>()
                                    .Where(i => i.JournalId == journal.Id)
                                    .Where(i => i.Category.ToLower().Trim().Equals(_category4.ToLower().Trim()))
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
                                    Year = readModel.Year
                                });
                            }

                            if (string.IsNullOrWhiteSpace(_category5) == false)
                            {
                                if (_category5.Contains("(Q") == false)
                                    continue;

                                var _catergory = _category5.Trim().Substring(0, _category5.Length - 5);
                                var _rank = _category5.Trim().Substring(_category5.Length - 5).Replace("(", "")
                                    .Replace(")", "").Trim();

                                var recordDup = _db.Query<JournalRecord>()
                                    .Where(i => i.JournalId == journal.Id)
                                    .Where(i => i.Category.ToLower().Trim().Equals(_category5.ToLower().Trim()))
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
                                    Year = readModel.Year
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

public class Scopus2Model
{
    public string? Title { get; set; }
    public object? ISSN { get; set; }
    public object? Country { get; set; }
    public string? Publisher { get; set; }
    public object? Category1 { get; set; }
    public object? Category2 { get; set; }
    public object? Category3 { get; set; }
    public object? Category4 { get; set; }
    public object? Category5 { get; set; }
}