using Entities;
using Entities.Journals;
using Entities.Models;
using Framework;
using Microsoft.AspNetCore.Mvc;
using UseCases;
using UseCases.Journals;
using Web.RazorPages;

namespace Banks.Pages._App.Journals
{
    [RequestFormLimits(MultipartBodyLengthLimit = 1048576000)]
    public class UpdateIF : AppPageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private IExcelFileReader _excelFileReader;
        private readonly IAddJournalRecord _addJournalRecord;
        private readonly IAddJournal _addJournal;
        public Dictionary<string, int> Indexes { get; set; }
        public ReadModel ReadModel { get; set; }

        public UpdateIF(IUnitOfWork unitOfWork, IExcelFileReader excelFileReader, IAddJournalRecord addJournalRecord,
            IAddJournal addJournal)
        {
            _unitOfWork = unitOfWork;
            _excelFileReader = excelFileReader;
            _addJournalRecord = addJournalRecord;
            _addJournal = addJournal;
            Indexes = EnumExt.GetList<JournalIndex>();
        }

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
                        var items = dataSet.SetToList<Journal_Model>();

                        foreach (var item in items)
                        {
                            if (string.IsNullOrEmpty(item.Title) == true)
                                continue;

                            var categories = item.Category.Split(";");

                            var journal = _unitOfWork.Journals.GetAll().FilterByTitle(item.Title).FirstOrDefault();

                            if (journal != null)
                            {
                                foreach (var cat in categories)
                                {
                                    var category = cat.Substring(0, cat.Length - 6).Trim();

                                    var records = _unitOfWork.JournalRecords.GetAll()
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
                                        _addJournalRecord.Respond(new IAddJournalRecord.Request
                                        {
                                            JournalId = journal.Id,
                                            Category = category,
                                            Year = readModel.Year,
                                            If = item.IF,
                                            QRank = GetQrank(item.QRank),
                                            Index = readModel.Index,
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var journalNew = _addJournal.Responce(new IAddJournal.Request
                                {
                                    Title = item.Title.Trim(),
                                    Issn = item.ISSN.Replace("-", ""),
                                    EIssn = item.EISSN.Replace("-", "")
                                });
                                _unitOfWork.Save();

                                foreach (var cat in categories)
                                {
                                    var category = cat.Substring(0, cat.Length - 6).Trim();

                                    _addJournalRecord.Respond(new IAddJournalRecord.Request
                                    {
                                        JournalId = journalNew.Id,
                                        Category = category,
                                        Year = readModel.Year,
                                        If = item.IF,
                                        QRank = GetQrank(item.QRank),
                                        Index = readModel.Index,
                                    });
                                }
                            }

                            _unitOfWork.Save();
                        }
                    }
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

            SuccessMessage = "با موفقیت اپدیت شد";
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

    public class Journal_Model
    {
        public string Title { get; set; }
        public string ISSN { get; set; }
        public string EISSN { get; set; }
        public string Category { get; set; }
        public string QRank { get; set; }
        public decimal? IF { get; set; }
    }
}