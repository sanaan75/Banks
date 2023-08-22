using Entities;
using Entities.Conferences;
using Entities.Utilities;
using Microsoft.AspNetCore.Mvc;
using UseCases.Interfaces;
using Web.Models.Conferences;

namespace Banks.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConferenceController : ApplicationController
{
    private readonly IDb _db;

    public ConferenceController(IDb db)
    {
        _db = db;
    }

    [Route("Add")]
    [HttpPost]
    public IActionResult Add(ConferenceModel model)
    {
        if (ModelState.IsValid == false)
            return BadRequest("model is not valid");

        var dup = _db.Query<Conference>().FilterByTitle(model.Title).Any();

        if (dup == true)
            return BadRequest("already exist");

        dup = _db.Query<Conference>().FilterByTitleEn(model.TitleEn).Any();

        if (dup == true)
            return BadRequest("already exist");

        if (model.Start > model.End)
            return BadRequest("start can not be after end");

        _db.Set<Conference>().Add(new Conference
        {
            Title = model.Title.ReplaceArabicLetters(),
            TitleEn = model.TitleEn.ReplaceArabicLetters(),
            Start = model.Start,
            End = model.End,
            Country = model.Country,
            CountryEn = model.CountryEn,
            City = model.City,
            CityEn = model.CityEn,
            //Level = model.Level,
            Organ = model.Organ,
            OrganEn = model.OrganEn,
            Customer = GetCustomer(model.Customer),
        });
        return Ok();
    }

    [Route("Search")]
    [HttpGet]
    public IActionResult Search(string key)
    {
        if (key.Length < 8)
            return BadRequest("key is very short");
        var result = _db.Query<Conference>().FilterByKeyword(key).ToList();
        return Ok(result);
    }

    [Route("FindByTitle")]
    [HttpGet]
    public IActionResult FindByTitle(string title, string? titleEn)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(titleEn))
                return BadRequest("title and titleEn can not be null");
            IQueryable<ConferenceModel> result;

            if (string.IsNullOrWhiteSpace(title) == false)
            {
                result = _db.Query<Conference>().FilterByKeyword(title).Select(i => new ConferenceModel
                {
                    Title = i.Title,
                    TitleEn = i.TitleEn,
                    Country = i.Country,
                    CountryEn = i.CountryEn,
                    Start = i.Start,
                    End = i.End,
                    Level = i.Level.GetCaption(),
                    City = i.City,
                    CityEn = i.CityEn,
                    Organ = i.Organ,
                    OrganEn = i.OrganEn,
                    Customer = i.Customer.GetCaption()
                });
            }
            else
                result = _db.Query<Conference>().FilterByKeyword(titleEn).Select(i => new ConferenceModel
                {
                    Title = i.Title,
                    TitleEn = i.TitleEn,
                    Country = i.Country,
                    CountryEn = i.CountryEn,
                    Start = i.Start,
                    End = i.End,
                    Level = i.Level.GetCaption(),
                    City = i.City,
                    CityEn = i.CityEn,
                    Organ = i.Organ,
                    OrganEn = i.OrganEn,
                    Customer = i.Customer.GetCaption()
                });

            return Ok(result.ToList());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private Customer? GetCustomer(string customer)
    {
        switch (customer.ToLower())
        {
            case "uok":
                return Customer.UOK;
            case "basu":
                return Customer.Basu;
            case "umz":
                return Customer.UMZ;
            default:
                return null;
        }
    }
    
}