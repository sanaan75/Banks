namespace Entities.Permissions;

public class UserGroup : IEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int Id { get; set; }
}