using Entities.Permissions;

namespace Entities;

public class Actor
{
    public bool IsAuthenticated { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; }
    public bool IsSysAdmin { get; set; }
    public List<Permission> Permissions { get; set; }
}