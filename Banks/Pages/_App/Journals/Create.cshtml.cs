using Entities.Journals;
using Framework;
using Microsoft.AspNetCore.Mvc;
using UseCases.Journals;
using Web.Models.Journals;
using Web.RazorPages;

namespace Banks.Pages._App.Journals
{
    public class Create : AppPageModel
    {
        private readonly IAddJournal _addJournal;
        private readonly IUnitOfWork _unitOfWork;
        public AddJournalModel AddJournalModel { get; set; }

        public Create(IAddJournal addJournal, IUnitOfWork unitOfWork)
        {
            _addJournal = addJournal;
            _unitOfWork = unitOfWork;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost(AddJournalModel addJournalModel)
        {
            try
            {
                var duplicate = _unitOfWork.Journals.GetAll().FilterByTitle(addJournalModel.Title).Any();
                if (duplicate)
                {
                    ErrorMessage = "مجله ای با این عنوان ثبت شده است.";
                    return Page();
                }

                _addJournal.Responce(new IAddJournal.Request
                {
                    Title = addJournalModel.Title,
                    Issn = addJournalModel.ISSN,
                    WebSite = addJournalModel.Website,
                    Publisher = addJournalModel.Publisher,
                    Country = addJournalModel.Country
                });
                _unitOfWork.Save();

                ModelState.Clear();
                SuccessMessage = "مجله با موفقیت ثبت شد.";
            }
            catch (Exception ex)
            {
                ErrorMessage = "مجله ثبت نشد";
            }

            return Page();
        }
    }
}