using Entities;
using Entities.Permissions;

namespace UseCases;

public class CheckMenu : ICheckMenu
{
    private const string Selected = "navigation-active";

    public CheckMenu()
    {
    }

    public string Respond(Menu selected)
    {
        if (selected == AppSetting.SelectedMenu)
            return Selected;

        return "";
    }
}