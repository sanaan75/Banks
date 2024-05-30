using ClosedXML.Excel;
using Entities.Journals;
using Entities.Models;
using Entities.Utilities;
using Microsoft.AspNetCore.Mvc;
using UseCases.Interfaces;
using Web.RazorPages;

namespace Banks.Pages._App.Journals;

[RequestFormLimits(MultipartBodyLengthLimit = 1048576000)]
public class ImportExcel : AppPageModel
{
    private readonly IDb _db;

    public ImportExcel(IDb db)
    {
        _db = db;
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
            ErrorMessage = "لطفا مقادیر خواسته شده را تکمیل نمایید.";
            return RedirectToPage();
        }

        try
        {
            if (readModel.FormFile.Length > 0)
            {
                var ms = new MemoryStream();
                readModel.FormFile.CopyTo(ms);
                using var wbook = new XLWorkbook(ms);

                var worksheet = wbook.Worksheet(1);

                var journals = _db.Query<Journal>()
                    .Select(i => new
                    {
                        i.Id,
                        i.NormalizedTitle,
                        i.Issn,
                        i.EIssn
                    }).ToList();

                foreach (var row in worksheet.Rows())
                {
                    try
                    {
                        var text = string.Empty;
                        var data = row.Cells();
                        foreach (var cell in data) text += cell.Value;

                        var sampleText = text.Replace('\"', ' ')
                            .Replace("&amp;", "-")
                            .Replace("&current;", "-");

                        var lastIndex = sampleText.LastIndexOf(')');
                        sampleText = sampleText.Substring(0, lastIndex + 1);

                        string[] columns = sampleText.Split(';');

                        if (columns[3].Trim().Equals("journal"))
                        {
                            List<string> catList = new();

                            for (int i = 22; i <= columns.Length - 1; i++)
                                catList.Add(columns[i]);

                            foreach (var category in catList)
                            {
                                string issn = null;
                                string eIssn = null;

                                var iisnText = columns[4].Trim().Replace("\"", "").Replace(" ", "");

                                if (iisnText.Length >= 8)
                                {
                                    eIssn = iisnText.Substring(0, 8);
                                    if (iisnText.Length == 16) issn = iisnText.Substring(8, 8);

                                    if (iisnText.Length >= 16) issn = iisnText.Substring(iisnText.Length - 8, 8);
                                }

                                if (GetCategory(category) is null)
                                    continue;

                                var model = new DataItem
                                {
                                    JournalName = columns[2].Trim().Replace("\"", ""),
                                    ISSN = issn.CleanIssn(),
                                    EISSN = eIssn.CleanIssn(),
                                    Year = readModel.Year,
                                    Index = readModel.Index,
                                    Category = GetCategory(category).Title,
                                    Rank = GetCategory(category).Rank,
                                    Country = columns[18].Trim().Replace("\"", ""),
                                    Publisher = columns[20].Trim().Replace("\"", "")
                                };

                                var journal = journals.FirstOrDefault(i =>
                                    i.NormalizedTitle.Equals(model.JournalName.VacuumString()) ||
                                    i.Issn == issn.CleanIssn() || i.EIssn == model.EISSN.CleanIssn());

                                if (journal != null)
                                {
                                    var historyAlreadyExists = _db.Query<JournalRecord>()
                                        .Where(i => i.JournalId == journal.Id)
                                        .Where(i => i.NormalizedCategory.Equals(category.VacuumString()))
                                        .Any(i => i.Year == model.Year);

                                    if (historyAlreadyExists)
                                        continue;

                                    _db.Set<JournalRecord>()
                                        .Add(new JournalRecord
                                        {
                                            JournalId = journal.Id,
                                            Year = model.Year,
                                            Category = model.Category,
                                            NormalizedCategory = model.Category.VacuumString(),
                                            Index = model.Index,
                                            QRank = model.Rank,
                                            If = model.IF,
                                            Source = JournalSource.SJR,
                                            Mif = null,
                                            Aif = null,
                                            Type = null,
                                            Value = null,
                                        });
                                }
                                else
                                {
                                    var newJournal = _db.Set<Journal>().Add(new Journal
                                    {
                                        Title = model.JournalName,
                                        NormalizedTitle = model.JournalName.VacuumString(),
                                        Issn = model.ISSN.CleanIssn(),
                                        EIssn = model.EISSN.CleanIssn(),
                                        Country = model.Country,
                                        Publisher = model.Publisher
                                    }).Entity;

                                    _db.Set<JournalRecord>().Add(new JournalRecord
                                    {
                                        Journal = newJournal,
                                        Year = model.Year,
                                        Index = model.Index,
                                        QRank = model.Rank,
                                        If = model.IF,
                                        Category = model.Category,
                                        NormalizedCategory = model.Category.VacuumString(),
                                        Source = JournalSource.SJR,
                                        Mif = null,
                                        Aif = null,
                                        Type = null,
                                        Value = null,
                                    });
                                    _db.Save();

                                    journals.Add(new
                                    {
                                        Id = newJournal.Id,
                                        NormalizedTitle = newJournal.NormalizedTitle,
                                        Issn = model.ISSN,
                                        EIssn = model.EISSN
                                    }!);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                _db.Save();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "عملیات با خطا مواجه شد" + ex.Message;
        }

        SuccessMessage = "با موفقیت اپدیت شد";
        return RedirectToPage();
    }

    private CategoryModel? GetCategory(string category)
    {
        if (category.Length > 5)
        {
            var rank = category.Substring(category.Length - 3, 2);
            var QRank = GetQrank(rank);

            var title = rank.StartsWith("Q")
                ? category.Substring(0, category.Length - 4).Trim()
                : category;
            return new CategoryModel
            {
                Title = title,
                Rank = QRank
            };
        }

        return null;
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

    public class DataItem
    {
        public string JournalName { get; set; }
        public string ISSN { get; set; }
        public string EISSN { get; set; }
        public int Year { get; set; }
        public JournalIndex Index { get; set; }
        public string Category { get; set; }
        public string Country { get; set; }
        public string Publisher { get; set; }
        public decimal? IF { get; set; }
        public JournalQRank? Rank { get; set; }
    }
}