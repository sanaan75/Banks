using Entities.Users;

namespace Entities.Permissions;

public class UserInGroup : IEntity
{
    public int UserId { get; set; }
    public User User { get; set; }
    public int UserGroupId { get; set; }
    public UserGroup UserGroup { get; set; }
    public int Id { get; set; }
}