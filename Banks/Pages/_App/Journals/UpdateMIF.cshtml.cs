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
                        var dataSet = _excelFileReader.ToDataSet(readModel.FormFile);
                        var list = dataSet.SetToList<MIFModel>();

                        foreach (var item in list)
                        {
                            var categories = item.Category.Split(",");
                            foreach (var category in categories)
                            {
                                var records = _unitOfWork.JournalRecords.GetAll().FilterByCategory(category.Trim())
                                    .FilterByYear(readModel.Year)
                                    .FilterByIndex(readModel.Index);

                                if (records.Any())
                                {
                                    foreach (var record in records)
                                    {
                                        record.Mif = item.MIF;
                                        record.Aif = item.AIF;
                                    }

                                    _unitOfWork.Save();
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
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

        public class MIFModel
        {
            public string Category { get; set; }
            public decimal? MIF { get; set; }
            public decimal? AIF { get; set; }
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