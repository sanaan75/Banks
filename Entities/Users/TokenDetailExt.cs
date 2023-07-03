namespace Entities.Users;

public static class TokenDetailExt
{
    public static IQueryable<TokenDetail> FilterDeactive(this IQueryable<TokenDetail> query)
    {
        return query.Where(detail => !detail.IsActive);
    }
}