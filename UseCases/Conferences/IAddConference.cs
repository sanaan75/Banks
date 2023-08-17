using Entities.Conferences;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace UseCases.Conferences;

public interface IAddConference
{
    EntityEntry<Conference> Respond(Request request);

    class Request
    {
        public string Title { get; set; }
        public string TitleEn { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Level Level { get; set; }
        public string Country { get; set; }
        public string CountryEn { get; set; }
        public string City { get; set; }
        public string CityEn { get; set; }
        public string Organ { get; set; }
        public string OrganEn { get; set; }
    }
}