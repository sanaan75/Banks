using Entities;
using Entities.Journals;
using Framework;
using Microsoft.AspNetCore.Mvc;
using UseCases;
using UseCases.Journals;
using Web.Models.Journals;
using Web.RazorPages;

namespace Banks.Pages._App.Journals
{
    [RequestFormLimits(MultipartBodyLengthLimit = 1048576000)]
    public class ReadExcelCloritive : AppPageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private IExcelFileReader _excelFileReader;
        private readonly IAddJournalRecord _addJournalRecord;
        private readonly IAddJournal _addJournal;
        public Dictionary<string, int> Indexes { get; set; }
        public ReadModel ReadModel { get; set; }

        public ReadExcelCloritive(IUnitOfWork unitOfWork, IExcelFileReader excelFileReader, IAddJournalRecord addJournalRecord,
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
                        
                        
                        
                        
                        var data = _excelFileReader.ToDataTable(readModel.FormFile);

                        string category = string.Empty;
                        for (int i = 0; i <= data.Rows.Count; i++)
                        {
                            if (data.Rows[i][0].ToString().Trim().Length > 2)
                            {
                                category = data.Rows[i][5].ToString().Trim();
                                var cats = category.Split(";");

                                var journal = _unitOfWork.Journals.Find(j =>
                                    j.Title.ToLower() == data.Rows[i][0].ToString().Trim().ToLower());

                                if (journal.Any())
                                {
                                    var journalData = journal.Select(i => new { i.Id }).First();

                                    foreach (var cat in cats)
                                    {
                                        var finalCategory = cat.Replace("- SCIE", " ");
                                        var dup = _unitOfWork.JournalRecords.GetAll().FilterByJournal(journalData.Id)
                                            .FilterByCategory(finalCategory).FilterByYear(readModel.Year).Any();

                                        if (dup == false)
                                        {
                                            _addJournalRecord.Respond(new IAddJournalRecord.Request
                                            {
                                                JournalId = journalData.Id,
                                                Category = finalCategory,
                                                If = Convert.ToDecimal(data.Rows[i][4].ToString()),
                                                Year = readModel.Year,
                                                Index = readModel.Index,
                                                QRank = GetQrank(data.Rows[i][3].ToString().Trim())
                                            });
                                            _unitOfWork.Save();
                                        }
                                    }
                                }

                                else
                                {
                                    var journalNew = _addJournal.Responce(new IAddJournal.Request
                                    {
                                        Title = data.Rows[i][0].ToString().Trim(),
                                        Issn = data.Rows[i][1].ToString().Trim(),
                                        EIssn = data.Rows[i][2].ToString().Trim()
                                    });

                                    _unitOfWork.Save();

                                    foreach (var cat in cats)
                                    {
                                        var finalCategory = cat.Replace("-SCIE", "");
                                        _addJournalRecord.Respond(new IAddJournalRecord.Request
                                        {
                                            JournalId = journalNew.Id,
                                            Category = finalCategory,
                                            If = Convert.ToDecimal(data.Rows[i][4].ToString()),
                                            Year = readModel.Year,
                                            Index = readModel.Index,
                                            QRank = GetQrank(data.Rows[i][3].ToString().Trim())
                                        });
                                    }

                                    _unitOfWork.Save();
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    ErrorMessage = "عملیات با خطا مواجه شد" + ex.Message;
                }
            }
            else
            {
                ErrorMessage = "لطفا مقادیر خواسته شده را تکمیل نمایید.";
            }

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
    }

}