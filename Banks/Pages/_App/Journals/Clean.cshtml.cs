using Entities.Journals;
using Entities.Models;
using Entities.Utilities;
using Microsoft.AspNetCore.Mvc;
using UseCases.Interfaces;
using Web.RazorPages;

namespace Banks.Pages._App.Journals;

[RequestFormLimits(MultipartBodyLengthLimit = 1048576000)]
public class Clean : AppPageModel
{
    private readonly IDb _db;

    public Clean(IDb db)
    {
        _db = db;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost(ReadModel readModel)
    {
        try
        {
            var journals = _db.Set<Journal>().ToList();
            foreach (var journal in journals)
            {
                journal.Title = journal.Title.Clean();
            }

            _db.Save();

            var categoies = _db.Set<JournalRecord>().ToList();
            foreach (var item in categoies)
            {
                item.Category = item.Category.Clean();
            }

            _db.Save();

            SuccessMessage = "با موفقیت انجام شد";
        }
        catch (Exception ex)
        {
            ErrorMessage = "عملیات با خطا مواجه شد";
        }

        return Page();
    }
}