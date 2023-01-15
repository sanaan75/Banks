using Entities;
using Entities.Journals;
using Framework;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using UseCases.Journals;
using Web.Models.Records;
using Web.RazorPages;

namespace JournalBank.Pages._App.Journals.Records
{
    public class Create : AppPageModel
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IAddJournalRecord _addJournalRecord;
        public AddRecordModel RecordModel { get; set; }
        public Dictionary<string, int> Indexes { get; set; }
        public Dictionary<string, int> Types { get; set; }
        public Dictionary<string, int> Ranks { get; set; }
        public int JournalId { get; set; }

        public Create(IUnitOfWork unitOfWork, IAddJournalRecord addJournalRecord)
        {
            _unitOfWork = unitOfWork;
            _addJournalRecord = addJournalRecord;
            Indexes = EnumExt.GetList<JournalIndex>();
            Types = EnumExt.GetList<JournalType>();
            Ranks = EnumExt.GetList<JournalQRank>();
        }

        public void OnGet(int journalId)
        {
            JournalId = journalId;
        }

        public IActionResult OnPost(AddRecordModel recordModel)
        {
            try
            {
                var journalRecord = _addJournalRecord.Respond(new IAddJournalRecord.Request
                {
                    JournalId = recordModel.JournalId,
                    Year = recordModel.Year,
                    Category = recordModel.Category,
                    Index = (JournalIndex)recordModel.Index,
                    JournalType = (JournalType)recordModel.Type,
                    QRank = (JournalQRank)recordModel.QRank,
                    If = recordModel.IF,
                    Mif = recordModel.MIF,
                    Aif = recordModel.AIF                    
                });
                _unitOfWork.Save();
                SuccessMessage = "رکرد با موفقیت اضافه شد.";
                ModelState.Clear();
                JournalId = recordModel.JournalId;
            }
            catch
            {
                ErrorMessage = "عملیات با خطا مواجه شد";
            }
            return Page();
        }
    }
}
