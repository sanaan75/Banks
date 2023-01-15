namespace Entities;

public static class FileContentExt
{
    public static IQueryable<FileContent> FilterByCode(this IQueryable<FileContent> items, int? id)
    {
        return items.Filter(id, i => i.Id == id.Value);
    }
}