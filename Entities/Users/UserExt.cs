namespace Entities.Users;

public static class UserExt
{
    public static User GetByUsername(this IQueryable<User> items, string username)
    {
        return items.FirstOrDefault(i => i.Username.Equals(username))!;
    }
}