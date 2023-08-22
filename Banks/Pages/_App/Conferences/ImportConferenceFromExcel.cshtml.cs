using Entities;
using Entities.Conferences;
using Entities.Journals;
using Entities.Models;
using Entities.Utilities;
using Microsoft.AspNetCore.Mvc;
using UseCases;
using UseCases.Interfaces;
using Web.RazorPages;

namespace Banks.Pages._App.Conferences;

[RequestFormLimits(MultipartBodyLengthLimit = 1048576000)]
public class ImportConferenceFromExcel : AppPageModel
{
    private readonly IDb _db;
    private IExcelFileReader _excelFileReader;

    public ImportConferenceFromExcel(IDb db, IExcelFileReader excelFileReader)
    {
        _db = db;
        _excelFileReader = excelFileReader;
        Indexes = EnumExt.GetList<JournalIndex>();
    }

    public Dictionary<string, int> Indexes { get; set; }
    public FileModel FileModel { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost(FileModel fileModel)
    {
        if (ModelState.IsValid)
            try
            {
                if (fileModel.FormFile.Length > 0)
                {
                    var dataSet = _excelFileReader.ToDataSet(fileModel.FormFile);
                    var list = dataSet.SetToList<DataItem>();

                    foreach (var item in list)
                    {
                        var dup = _db.Query<Conference>().Any(i =>
                            i.Title.ToLower().Trim().Replace(" ", "")
                                .Equals(item.Title.ToLower().Trim().Replace(" ", "")));

                        if (dup == true)
                            continue;

                        _db.Set<Conference>().Add(new Conference
                        {
                            Title = item.Title,
                            TitleEn = item.TitleEn,
                            Country = item.Country,
                            CountryEn = item.CountryEn,
                            Start = item.Start,
                            End = item.End,
                            City = item.City,
                            CityEn = item.CityEn,
                            Organ = item.Organ,
                            OrganEn = item.OrganEn,
                            Level = (Level)item.Level.ToInt32(),
                            Customer = Customer.Public
                        });
                    }
                    _db.Save();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "عملیات با خطا مواجه شد" + ex.Message;
            }
        else
            ErrorMessage = "لطفا مقادیر خواسته شده را تکمیل نمایید.";

        SuccessMessage = "با موفقیت اپدیت شد";
        return Page();
    }

    public class DataItem
    {
        public string Title { get; set; }
        public string TitleEn { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public double Level { get; set; }
        public string Country { get; set; }
        public string CountryEn { get; set; }
        public string City { get; set; }
        public string CityEn { get; set; }
        public string Organ { get; set; }
        public string OrganEn { get; set; }
        public double Customer { get; set; }
    }
}