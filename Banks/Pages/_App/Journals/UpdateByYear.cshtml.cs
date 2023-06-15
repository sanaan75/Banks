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
    public class UpdateByYear : AppPageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private IExcelFileReader _excelFileReader;
        private readonly IAddJournalRecord _addJournalRecord;
        private readonly IAddJournal _addJournal;
        public Dictionary<string, int> Indexes { get; set; }
        public ReadModel ReadModel { get; set; }

        public UpdateByYear(IUnitOfWork unitOfWork, IExcelFileReader excelFileReader,
            IAddJournalRecord addJournalRecord,
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
                int row = 1;
                string title_ = "";
                try
                {
                    if (readModel.FormFile.Length > 0)
                    {
                        var dataSet = _excelFileReader.ToDataSet(readModel.FormFile);
                        var items = dataSet.SetToList<Model>();
                        var journals = _unitOfWork.Journals.GetAll().Select(i => new { i.Id, i.Title }).ToList();
                        try
                        {
                            foreach (var item in items)
                            {
                                if (string.IsNullOrEmpty(item.Title) == true)
                                    continue;

                                if (item.Categories.Equals("N/A"))
                                    continue;

                                var categories = item.Categories.Split(",");

                                var journal = journals.FirstOrDefault(i =>
                                    i.Title.Trim().ToLower() == item.Title.Trim().ToLower());

                                if (journal != null)
                                {
                                    foreach (var cat in categories)
                                    {
                                        var category = cat.Substring(0, cat.Length - 5).Trim();

                                        var records = _unitOfWork.JournalRecords.GetAll()
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
                                                string qRank = cat.Substring(startIndex).Trim();
                                                qRank = qRank.Replace("(", "").Replace(")", "").Trim();
                                                record.QRank = GetQrank(qRank);
                                            }
                                        }
                                        else
                                        {
                                            string qRank = string.Empty;
                                            if (cat.Contains("("))
                                            {
                                                var startIndex = cat.IndexOf('(');
                                                qRank = cat.Substring(startIndex).Trim();
                                                qRank = qRank.Replace("(", "").Replace(")", "").Trim();
                                            }

                                            var historyAlreadyExists = _unitOfWork.JournalRecords
                                                .Find(i => i.JournalId == journal.Id)
                                                .FilterByCategory(category)
                                                .Any(i => i.Year == readModel.Year);

                                            if (historyAlreadyExists == true)
                                                continue;

                                            _unitOfWork.JournalRecords.Add(new JournalRecord
                                            {
                                                JournalId = journal.Id,
                                                Year = readModel.Year,
                                                Index = readModel.Index,
                                                Type = null,
                                                Value = null,
                                                IscClass = null,
                                                QRank = GetQrank(qRank),
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

                                    foreach (var cat in categories)
                                    {
                                        var qRank = String.Empty;
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

                                        _unitOfWork.JournalRecords.Add(new JournalRecord
                                        {
                                            Journal = _newJournal,
                                            Year = readModel.Year,
                                            Index = readModel.Index,
                                            Type = null,
                                            Value = null,
                                            IscClass = null,
                                            QRank = GetQrank(qRank),
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

                            _unitOfWork.Save();
                        }
                        catch (Exception ex)
                        {
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
        public decimal? IF { get; set; }
        public string Categories { get; set; }
    }
}