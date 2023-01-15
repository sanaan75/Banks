namespace Web;

public static class PageUrlExt
{
    private static Dictionary<string, string> _pageUrl;

    public static string Get(string key)
    {
        if (_pageUrl == null)
        {
            _pageUrl = new Dictionary<string, string>();
            var fields = typeof(PageUrl).GetFields();

            foreach (var field in fields)
                _pageUrl.Add(field.Name, field.GetValue(null) as string);
        }

        try
        {
            if (key == null)
                return string.Empty;

            return _pageUrl[key];
        }
        catch
        {
            return string.Empty;
        }
    }
}