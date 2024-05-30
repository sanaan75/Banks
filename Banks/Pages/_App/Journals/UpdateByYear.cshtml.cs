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
    private readonly IDb _db;
    private IExcelFileReader _excelFileReader;

    public UpdateByYear(IDb db, IExcelFileReader excelFileReader, IAddJournal addJournal)
    {
        _db = db;
        _excelFileReader = excelFileReader;
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
        if (ModelState.IsValid == false)
        {
            ErrorMessage = "invalid file";
            return RedirectToPage();
        }

        try
        {
            if (readModel.FormFile.Length > 0)
            {
                var dataSet = _excelFileReader.ToDataSet(readModel.FormFile);
                var items = dataSet.SetToList<DataModel>();

                var journals = _db.Query<Journal>()
                    .Select(i => new
                    {
                        i.Id,
                        i.NormalizedTitle
                    }).ToList();

                try
                {
                    foreach (var item in items)
                    {
                        if (string.IsNullOrWhiteSpace(item.Title.Trim()))
                            continue;

                        if (item.Categories.Trim().Equals("N/A"))
                            continue;

                        var journal =
                            journals.FirstOrDefault(i => i.NormalizedTitle.Equals(item.Title.VacuumString()));

                        if (journal == null)
                        {
                            var _newJournal = _addJournal.Responce(new IAddJournal.Request
                            {
                                Title = item.Title.Trim(),
                                Issn = item.ISSN.CleanIssn(),
                            });

                            _db.Save();

                            journals.Add(new
                            {
                                _newJournal.Id,
                                _newJournal.NormalizedTitle
                            });
                        }

                        var categories = item.Categories.Split(",");

                        foreach (var category in categories)
                        {
                            var historyAlreadyExists = _db.Query<JournalRecord>()
                                .Where(i => i.JournalId == journal.Id)
                                .Where(i => i.NormalizedCategory.Equals(category.VacuumString()))
                                .Any(i => i.Year == readModel.Year);

                            if (historyAlreadyExists)
                                continue;

                            _db.Set<JournalRecord>().Add(new JournalRecord
                            {
                                JournalId = journal.Id,
                                Year = readModel.Year,
                                Category = category.Trim(),
                                NormalizedCategory = category.VacuumString(),
                                Index = readModel.Index,
                                QRank = GetQrank(item.Quartile),
                                If = item.IF.To<decimal>(),
                                Mif = item.MIF.To<decimal>(),
                                Aif = item.AIF.To<decimal>(),
                                Type = null,
                                Value = null,
                            });
                        }
                    }

                    _db.Save();
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

        return RedirectToPage();
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

    public class DataModel
    {
        public string Title { get; set; }
        public string ISSN { get; set; }
        public string Categories { get; set; }
        public string Quartile { get; set; }
        public double IF { get; set; }
        public double MIF { get; set; }
        public double AIF { get; set; }
    }
}