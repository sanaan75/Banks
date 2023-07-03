using ClosedXML.Excel;
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
public class ImportExcel : AppPageModel
{
    private readonly IAddJournal _addJournal;
    private readonly IAddJournalRecord _addJournalRecord;
    private readonly IDb _db;
    private IExcelFileReader _excelFileReader;

    public ImportExcel(IDb db, IExcelFileReader excelFileReader, IAddJournalRecord addJournalRecord,
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
                    var ms = new MemoryStream();
                    readModel.FormFile.CopyTo(ms);
                    using var wbook = new XLWorkbook(ms);

                    var worksheet = wbook.Worksheet(1);

                    var journals = _db.Query<Journal>()
                        .Select(i => new { i.Id, Title = i.Title.Trim().ToLower() });

                    foreach (var row in worksheet.Rows())
                    {
                        var text = string.Empty;
                        var data = row.Cells();
                        foreach (var cell in data) text += cell.Value;

                        var sampleText = text.Replace('\"', ' ');
                        sampleText = sampleText.Replace("&amp;", "-");
                        sampleText = sampleText.Replace("&current;", "-");

                        var columns = sampleText.Split(";");

                        var type = columns[3].Trim();
                        if (type.Equals("journal"))
                        {
                            var catText = string.Empty;

                            for (var k = 19; k < columns.Length; k++) catText += columns[k].Trim() + ";";

                            var categories = GetCategories(catText.Replace("\"", ""));
                            foreach (var category in categories)
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

                                var model = new DataItem
                                {
                                    JournalName = columns[2].Trim().Replace("\"", ""),
                                    ISSN = issn,
                                    EISSN = eIssn,
                                    Year = readModel.Year,
                                    Index = readModel.Index,
                                    Category = category.Title,
                                    Rank = category.Rank,
                                    Country = columns[15].Trim().Replace("\"", ""),
                                    Publisher = columns[17].Trim().Replace("\"", "")
                                };

                                var journal = journals.FirstOrDefault(i =>
                                    i.Title == model.JournalName.Trim().ToLower());

                                if (journal != null)
                                {
                                    var dup = _db.Query<JournalRecord>().FilterByJournal(journal.Id)
                                        .FilterByCategory(model.Category).FilterByYear(model.Year).Any();

                                    if (dup == false)
                                        _addJournalRecord.Respond(new IAddJournalRecord.Request
                                        {
                                            JournalId = journal.Id,
                                            Category = model.Category,
                                            If = model.IF,
                                            Year = model.Year,
                                            Index = model.Index,
                                            QRank = model.Rank
                                        });
                                }
                                else
                                {
                                    var newJournal = _addJournal.Responce(new IAddJournal.Request
                                    {
                                        Title = model.JournalName,
                                        Issn = model.ISSN,
                                        EIssn = model.EISSN,
                                        Country = model.Country,
                                        Publisher = model.Publisher
                                    });

                                    _db.Set<JournalRecord>().Add(new JournalRecord
                                    {
                                        Journal = newJournal,
                                        Year = model.Year,
                                        Index = model.Index,
                                        Type = null,
                                        Value = null,
                                        IscClass = null,
                                        QRank = model.Rank,
                                        If = model.IF,
                                        Category = model.Category,
                                        Mif = null,
                                        Aif = null,
                                        CreatorId = 1, //_actorService.UserId,
                                        CreateDate = DateTime.UtcNow
                                    });
                                }

                                _db.Save();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "عملیات با خطا مواجه شد" + ex.Message;
            }
        else
            ErrorMessage = "لطفا مقادیر خواسته شده را تکمیل نمایید.";

        SuccessMessage = "با موفقیت اپدیت شد";
        return Page();
    }

    private List<CategoryModel> GetCategories(string categories)
    {
        var items = new List<CategoryModel>();
        if (categories.Length > 5)
        {
            categories = categories.Substring(0, categories.Length - 1);
            var list = categories.Split(";");
            foreach (var item in list)
            {
                var category = item.Trim();
                var rank = category.Substring(category.Length - 3, 2);
                var QRank = GetQrank(rank);

                var title = rank.StartsWith("Q")
                    ? category.Substring(0, category.Length - 4).Trim()
                    : category;
                items.Add(new CategoryModel
                {
                    Title = title,
                    Rank = QRank
                });
            }
        }

        return items;
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