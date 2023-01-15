using ClosedXML.Excel;
using Entities;
using Entities.Journals;
using Framework;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UseCases;
using UseCases.Journals;
using Web.Models.Journals;
using Web.RazorPages;

namespace JournalBank.Pages._App.Journals
{
    [RequestFormLimits(MultipartBodyLengthLimit = 1048576000)]
    public class ReadExcel : AppPageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private IExcelFileReader _excelFileReader;
        private readonly IAddJournalRecord _addJournalRecord;
        private readonly IAddJournal _addJournal;
        public Dictionary<string, int> Indexes { get; set; }
        public ReadModel ReadModel { get; set; }

        public ReadExcel(IUnitOfWork unitOfWork, IExcelFileReader excelFileReader, IAddJournalRecord addJournalRecord,
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
                        MemoryStream ms = new MemoryStream();
                        readModel.FormFile.CopyTo(ms);
                        using var wbook = new XLWorkbook(ms);

                        var worksheet = wbook.Worksheet(1);

                        foreach (var row in worksheet.Rows())
                        {
                            string text = string.Empty;
                            var data = row.Cells();
                            foreach (var cell in data)
                            {
                                text += cell.Value;
                            }

                            var sampleText = text.Replace('\"', ' ');
                            sampleText = text.Replace("&amp;", "-");
                            sampleText = text.Replace("&current;", "-");

                            var finalText = sampleText.Split(";");

                            var type = finalText[3].Trim();
                            if (type.Equals("journal") == true)
                            {
                                string catText = string.Empty;

                                for (int k = 19; k < finalText.Length; k++)
                                {
                                    catText += finalText[k].Trim() + ";";
                                }

                                var categories = GetCategories(catText.Replace("\"", ""));
                                foreach (var category in categories)
                                {
                                    string issn = null;
                                    string eIssn = null;

                                    var iisnText = finalText[4].Trim().Replace("\"", "").Replace(" ", "");

                                    if (iisnText.Length >= 8)
                                    {
                                        eIssn = iisnText.Substring(0, 8);
                                        if (iisnText.Length == 16)
                                        {
                                            issn = iisnText.Substring(8, 8);
                                        }
                                        if (iisnText.Length >= 16)
                                        {
                                            issn = iisnText.Substring(iisnText.Length - 8, 8);
                                        }
                                    }

                                    var model = new JournalRecordModel
                                    {
                                        JournalName = finalText[2].Trim().Replace("\"", ""),
                                        ISSN = issn,
                                        EISSN = eIssn,
                                        Year = readModel.Year,
                                        Index = readModel.Index,
                                        Category = category.Title,
                                        Rank = category.Rank,
                                        Country = finalText[15].Trim().Replace("\"", ""),
                                        Publisher = finalText[17].Trim().Replace("\"", "")
                                    };

                                    var journal = _unitOfWork.Journals.Find(i =>
                                        i.Title.ToLower() == model.JournalName.ToLower());

                                    if (journal.Any())
                                    {
                                        var journalData = journal.Select(i => new { i.Id }).First();

                                        var dup = _unitOfWork.JournalRecords.GetAll().FilterByJournal(journalData.Id)
                                            .FilterByCategory(model.Category).FilterByYear(model.Year).Any();

                                        if (dup == false)
                                        {
                                            _addJournalRecord.Respond(new IAddJournalRecord.Request
                                            {
                                                JournalId = journalData.Id,
                                                Category = model.Category,
                                                If = model.IF,
                                                Year = model.Year,
                                                Index = model.Index,
                                                QRank = model.Rank
                                            });
                                        }
                                    }
                                    else
                                    {
                                        var journalNew = _addJournal.Responce(new IAddJournal.Request
                                        {
                                            Title = model.JournalName,
                                            Issn = model.ISSN,
                                            EIssn = model.EISSN,
                                            Country = model.Country,
                                            Publisher = model.Publisher
                                        });

                                        _unitOfWork.Save();

                                        _addJournalRecord.Respond(new IAddJournalRecord.Request
                                        {
                                            JournalId = journalNew.Id,
                                            Category = model.Category,
                                            If = model.IF,
                                            Year = model.Year,
                                            Index = model.Index,
                                            QRank = model.Rank
                                        });
                                    }

                                    _unitOfWork.Save();
                                }
                            }
                            else
                            {
                                // type is not journal
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