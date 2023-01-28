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
    public class UpdateMIF : AppPageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private IExcelFileReader _excelFileReader;
        private readonly IAddJournalRecord _addJournalRecord;
        private readonly IAddJournal _addJournal;
        public Dictionary<string, int> Indexes { get; set; }
        public ReadModel ReadModel { get; set; }

        public UpdateMIF(IUnitOfWork unitOfWork, IExcelFileReader excelFileReader, IAddJournalRecord addJournalRecord,
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

                        var categoryName = string.Empty;
                        decimal? MIF;
                        decimal? AIF;


                        for (int i = 0; i <= data.Rows.Count; i++)
                        {
                            var trim = data.Rows[i][0].ToString().Trim();

                            categoryName = trim.Replace("-SSCI", " ");
                            categoryName = categoryName.Trim();

                            var mif = data.Rows[i][1].ToString().Trim();
                            MIF = mif.Equals("N/A") ? null : Convert.ToDecimal(mif);

                            var aif = data.Rows[i][2].ToString().Trim();
                            AIF = (aif.Equals("N/A") || aif.Equals("")) ? null : Convert.ToDecimal(aif);

                            var records = _unitOfWork.JournalRecords.GetAll().FilterByCategory(categoryName)
                                .FilterByYear(readModel.Year)
                                .FilterByIndex(readModel.Index);

                            if (records.Any())
                            {
                                foreach (var record in records)
                                {
                                    record.Mif = MIF;
                                    record.Aif = AIF;
                                }

                                _unitOfWork.Save();
                            }
                        }
                    }
                }
                catch
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