using Entities.Permissions;

namespace Entities.Users;

public class User : IEntity
{
    public string Username { get; set; }
    public string Password { get; set; }

    public string? Name { get; set; }
    public string Title { get; set; }
    public int? UserGroupId { get; set; }
    public UserGroup UserGroup { get; set; }

    public bool Enabled { get; set; }
    public bool SysAdmin { get; set; }

    public string? NationalCode { get; set; }
    public string? University { get; set; }
    public int Id { get; set; }
}