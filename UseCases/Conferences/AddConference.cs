using Entities.Conferences;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UseCases.Interfaces;

namespace UseCases.Conferences;

public class AddConference : IAddConference
{
    private readonly IDb _db;

    public AddConference(IDb db)
    {
        _db = db;
    }

    public EntityEntry<Conference> Respond(IAddConference.Request request)
    {
        return _db.Set<Conference>().Add(new Conference
        {
            Title = request.Title,
            TitleEn = request.TitleEn,
            Country = request.Country,
            CountryEn = request.CountryEn,
            Start = request.Start,
            End = request.End,
            City = request.City,
            CityEn = request.CityEn,
            Level = request.Level,
            Organ = request.Organ,
            OrganEn = request.OrganEn
        });
    }
}