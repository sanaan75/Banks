using Entities.Helpers.TextHelper;
using System.Linq;

namespace Entities.Users
{
    public static class UserExt
    {
        public static IQueryable<User> FilterByKeyword(this IQueryable<User> items, string keyword)
        {
            var tokens = keyword.GetTokens();

            foreach (var token in tokens)
            {
                items = items.Where(i => i.Title.Contains(token) || i.Username.Contains(token));
            }

            return items;
        }
    }
}
