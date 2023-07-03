using System.ComponentModel.DataAnnotations;

namespace Web.Models.Journals
{
    public class JournalModel
    {
        [Display(Name = "شناسه")] public int Id { get; set; }

        [Required(ErrorMessage = "عنوان اجباری است")]
        [Display(Name = "عنوان مجله")]
        public string Title { get; set; }

        [Display(Name = "Issn")] public string ISSN { get; set; }

        [Display(Name = "وب سایت")] public string Website { get; set; }

        [Display(Name = "ناشر")] public string Publisher { get; set; }

        [Display(Name = "کشور")] public string Country { get; set; }
    }
}