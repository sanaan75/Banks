using System.ComponentModel.DataAnnotations;
using Entities.Journals;
using Microsoft.AspNetCore.Http;

namespace Entities.Models
{
    public class ReadModel
    {
        [Required(ErrorMessage = "سال اجباری است")]
        [Display(Name = "سال")]
        public int Year { get; set; }

        [Display(Name = "نمایه")]
        [Required(ErrorMessage = "نمایه اجباری است")]
        public JournalIndex Index { get; set; }

        [Display(Name = "فایل")]
        [Required(ErrorMessage = "فایل اجباری است")]
        public IFormFile FormFile { get; set; }
    }
}
