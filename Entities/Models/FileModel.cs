using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Entities.Models;

public class FileModel
{
    [Display(Name = "فایل")]
    [Required(ErrorMessage = "فایل اجباری است")]
    public IFormFile FormFile { get; set; }
}