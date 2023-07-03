using Entities;
using Entities.Journals;
using Entities.Permissions;
using Entities.Users;
using UseCases.Interfaces;
using Web;
using Web.Models;
using Web.RazorPages;

namespace Banks.Pages._App;

public class Index : AppPageModel
{
    private readonly IDb _db;
    private readonly IMenuHelper _menuHelper;

    public Index(IDb db, IMenuHelper menuHelper)
    {
        _db = db;
        _menuHelper = menuHelper;
        AppSetting.SelectedMenu = Menu.Home;
        _menuHelper.SetBreadcrumb(new List<string> { ResourceKey.Home });
    }

    public MainPageModel MainPageModel { get; set; }
    public int Journals { get; set; }
    public int Users { get; set; }

    public void OnGet()
    {
        Journals = _db.Query<Journal>().Count();
        Users = _db.Query<User>().Count();
    }
}