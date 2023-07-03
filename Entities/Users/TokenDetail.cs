namespace Entities.Users;

public class TokenDetail : IEntity
{
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime Expire { get; set; }
    public string State { get; set; }
    public bool IsActive { get; set; }
    public int Id { get; set; }
}