using System.ComponentModel.DataAnnotations;

namespace Web.Models.Conferences;

public class ConferenceModel
{
    [Required(ErrorMessage = "title is required")]
    public string Title { get; set; }
    public string TitleEn { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Level { get; set; }
    public string Country { get; set; }
    public string CountryEn { get; set; }
    public string City { get; set; }
    public string CityEn { get; set; }
    public string Organ { get; set; }
    public string OrganEn { get; set; }
    public string Customer { get; set; }
}