namespace Entities.Permissions;

public class UserGroupPermission : IEntity
{
    public int UserGroupId { get; set; }
    public UserGroup UserGroup { get; set; }

    public Permission Permission { get; set; }
    public int Id { get; set; }
}