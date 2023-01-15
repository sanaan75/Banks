using Entities;
using Entities.Permissions;
using Framework;
using Web;
using Web.Models;
using Web.RazorPages;

namespace Banks.Pages._App;

public class Index : AppPageModel
{
    private readonly IMenuHelper _menuHelper;
    private readonly IUnitOfWork _unitOfWork;

    public List<Note> Notes = new();

    public Index(IUnitOfWork unitOfWork, IMenuHelper menuHelper)
    {
        _unitOfWork = unitOfWork;
        _menuHelper = menuHelper;
        AppSetting.SelectedMenu = Menu.Home;
        _menuHelper.SetBreadcrumb(new List<string> { ResourceKey.Home });
    }

    public MainPageModel MainPageModel { get; set; }
    public int journals { get; set; }
    public int users { get; set; }

    public void OnGet()
    {
        journals = _unitOfWork.Journals.GetAll().Count();
        users = _unitOfWork.Users.GetAll().Count();
    }
}

public class Note
{
    public string text { get; set; }
    public string url { get; set; }
}