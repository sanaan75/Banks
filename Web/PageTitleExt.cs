namespace Web;

public static class PageTitleExt
{
    private static Dictionary<string, string> _pageTitles;

    public static string Get(string key)
    {
        if (_pageTitles == null)
        {
            _pageTitles = new Dictionary<string, string>();
            var fields = typeof(PageTitle).GetFields();

            foreach (var field in fields)
                _pageTitles.Add(field.Name, field.GetValue(null) as string);
        }

        try
        {
            if (key == null)
                return string.Empty;

            return _pageTitles[key];
        }
        catch
        {
            return key;
        }
    }
}