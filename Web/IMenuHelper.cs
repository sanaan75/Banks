using Entities.Permissions;
using Entities.Utilities;

namespace Web;

public interface IMenuHelper
{
    public string Respond(Menu selected);

    public List<BreadcrumbItem> GetBreadcrumb();
    public void SetBreadcrumb(List<string> items);
}