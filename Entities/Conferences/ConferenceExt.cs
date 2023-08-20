using Entities.Utilities;
using Entities.Utilities.TextHelper;

namespace Entities.Conferences;

public static class ConferenceExt
{
    public static IQueryable<Conference> FilterByKeyword(this IQueryable<Conference> items, string keyword)
    {
        var tokens = TextHelper.GetTokens(keyword);

        foreach (var token in tokens)
            items = items.Where(i =>
                i.Title.Trim().ToLower().Contains(token.ToLower()) ||
                i.TitleEn.Trim().ToLower().Contains(token.ToLower()));

        return items;
    }

    public static IQueryable<Conference> FilterByTitle(this IQueryable<Conference> items, string title)
    {
        return items.Filter(title,
            i => i.Title.Replace(" ", string.Empty).Trim().ToLower() ==
                 title.Replace(" ", string.Empty).Trim().ToLower());
    }

    public static IQueryable<Conference> FilterByTitleEn(this IQueryable<Conference> items, string title)
    {
        return items.Filter(title,
            i => i.TitleEn.Replace(" ", string.Empty).Trim().ToLower() ==
                 title.Replace(" ", string.Empty).Trim().ToLower());
    }
}