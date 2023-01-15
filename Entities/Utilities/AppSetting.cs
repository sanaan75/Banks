using Entities.Permissions;

namespace Entities;

public static class AppSetting
{
    public static Menu SelectedMenu { get; set; }
    public static List<BreadcrumbItem> Breadcrumb = new();
    public static string EmailTemplate = "EmailTemplate.html";
}

public class BreadcrumbItem
{
    public string Title { get; set; }
    public string URL { get; set; }
}