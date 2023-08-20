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
public class UpdateIF2 : AppPageModel
{
    private readonly IAddJournal _addJournal;
    private readonly IAddJournalRecord _addJournalRecord;
    private readonly IDb _unitOfWork;
    private IExcelFileReader _excelFileReader;

    public UpdateIF2(IDb unitOfWork, IExcelFileReader excelFileReader, IAddJournalRecord addJournalRecord,
        IAddJournal addJournal)
    {
        _unitOfWork = unitOfWork;
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
                    var items = dataSet.SetToList<JournalModel>();

                    foreach (var item in items)
                    {
                        if (string.IsNullOrEmpty(item.Title))
                            continue;

                        var journal = _unitOfWork.Query<Journal>().FilterByTitle(item.Title).FirstOrDefault();

                        if (journal != null)
                        {
                            var categories = _unitOfWork.Query<JournalRecord>().FilterByYear(readModel.Year)
                                .FilterByIndex(readModel.Index).FilterByJournal(journal.Id);

                            if (journal.Issn == null && string.IsNullOrEmpty(item.ISSN) == false)
                                journal.Issn = item.ISSN.Replace("-", "");

                            foreach (var cat in categories) cat.If = item.IF;
                        }
                    }

                    _unitOfWork.Save();
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

    public class JournalIfModel
    {
        public string Title { get; set; }
        public string ISSN { get; set; }
        public decimal? IF { get; set; }
    }

    public class JournalModel
    {
        public string Title { get; set; }
        public string ISSN { get; set; }
        public string EISSN { get; set; }
        public string Category { get; set; }
        public string QRank { get; set; }
        public decimal? IF { get; set; }
    }
}