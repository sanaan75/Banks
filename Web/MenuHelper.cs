using Entities;
using Entities.Permissions;

namespace Web;

public class MenuHelper : IMenuHelper
{
    private const string Selected = "navigation-active";

    public string Respond(Menu selected)
    {
        if (selected == AppSetting.SelectedMenu)
            return Selected;

        return "";
    }

    public List<BreadcrumbItem> GetBreadcrumb()
    {
        return AppSetting.Breadcrumb;
    }

    public void SetBreadcrumb(List<string> items)
    {
        if (AppSetting.Breadcrumb.Any())
            AppSetting.Breadcrumb.Clear();

        foreach (var item in items)
        {
            var breadcrumb = new BreadcrumbItem
            {
                Title = PageTitleExt.Get(item),
                URL = PageUrlExt.Get(item)
            };
            AppSetting.Breadcrumb.Add(breadcrumb);
        }
    }
}