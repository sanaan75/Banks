using System.Globalization;
using System.Text;

namespace Entities.Utilities;

public static class StringHelper
{
    public static IList<string> GetTokens(this string str)
    {
        return
            string.IsNullOrWhiteSpace(str)
                ? Array.Empty<string>()
                : str.Split(new[] { ' ', '\n', '\r', '\t', '-', ',', '،', '.', '?' },
                    StringSplitOptions.RemoveEmptyEntries);
    }

    public static string FirstLetter(this string str)
    {
        return string.IsNullOrWhiteSpace(str) ? string.Empty : str[0].ToString();
    }

    public static string Join(string separator, params string[] str)
    {
        return string.Join(separator, str.Where(i => string.IsNullOrWhiteSpace(i) == false));
    }

    public static string Join(string separator, IEnumerable<string> str)
    {
        return Join(separator, str.ToArray());
    }

    public static DateTime? GetDate(string date)
    {
        try
        {
            var dateArr = ToEnglishNumber(date).Split("/");

            var year = Convert.ToInt32(dateArr[0]);
            var month = Convert.ToInt32(dateArr[1]);
            var day = Convert.ToInt32(dateArr[2]);

            var dt = new DateTime(year, month, day, new PersianCalendar());
            return dt;
        }
        catch
        {
            return null;
        }
    }

    public static string ToEnglishNumber(string text)
    {
        try
        {
            if (text != null && text.Length == 10)
            {
                var LettersDictionary = new Dictionary<char, char>
                {
                    ['۰'] = '0',
                    ['۱'] = '1',
                    ['۲'] = '2',
                    ['۳'] = '3',
                    ['۴'] = '4',
                    ['۵'] = '5',
                    ['۶'] = '6',
                    ['۷'] = '7',
                    ['۸'] = '8',
                    ['۹'] = '9',
                    ['/'] = '/'
                };
                foreach (var item in text) text = text.Replace(item, LettersDictionary[item]);
            }

            return text;
        }
        catch
        {
            return string.Empty;
        }
    }

    public static int LevenshteinDistance(string s, string t)
    {
        if (s == t) return 0;
        if (s.Length == 0) return t.Length;
        if (t.Length == 0) return s.Length;
        var tLength = t.Length;
        var columns = tLength + 1;
        var v0 = new int[columns];
        var v1 = new int[columns];
        for (var i = 0; i < columns; i++)
            v0[i] = i;
        for (var i = 0; i < s.Length; i++)
        {
            v1[0] = i + 1;
            for (var j = 0; j < tLength; j++)
            {
                var cost = s[i] == t[j] ? 0 : 1;
                v1[j + 1] = Math.Min(Math.Min(v1[+j] + 1, v0[j + 1] + 1), v0[j] + cost);
                v0[j] = v1[j];
            }

            v0[tLength] = v1[tLength];
        }

        return v1[tLength];
    }

    public static bool StartsWith(string s1, string s2)
    {
        if (string.IsNullOrWhiteSpace(s1) || string.IsNullOrWhiteSpace(s2))
            return false;

        return s1.StartsWith(s2) || s2.StartsWith(s1);
    }

    public static string GetChange(string name, object oldValue, object newValue)
    {
        var oldValueString = oldValue?.ToString() ?? "-";
        var newValueString = newValue?.ToString() ?? "-";

        if (oldValueString == newValueString)
            return null;

        return $"{name} از «{oldValueString}» به «{newValueString}» تغییر کرد";
    }

    public static string Ellipsize(this string value, int maxChars)
    {
        return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
    }

    public static byte[] GetUtf8Bytes(string str)
    {
        var utf = new UTF8Encoding();
        return utf.GetBytes(str);
    }

    public static string GetUTF8String(byte[] bytes)
    {
        var utf = new UTF8Encoding();
        return utf.GetString(bytes);
    }

    public static string AddInBraces(this string text1, string text2)
    {
        return text2 == null ? text1 : $"{text1} ({text2})";
    }

    public static string ToStaticFileUrl(this string path)
    {
        var keyword = "/wwwroot/";
        var index = path.IndexOf(keyword);

        return path.Substring(index).Replace(keyword, "/");
    }

    public static bool ContainsUrl(this string fullUrl, string url)
    {
        var fullUrlSections = fullUrl.Split("/");
        var urlSections = url.Split("/");

        if (urlSections.Length < 1)
            return false;

        var result = false;
        var breakTheLoop = false;
        var currentFileUrlIndex = 0;
        foreach (var urlSection in urlSections)
        {
            //this is for not including the first empty method (because the split method sometimes creates the first member empty) 
            if (currentFileUrlIndex == 0 && string.IsNullOrEmpty(urlSection))
                continue;

            while (currentFileUrlIndex < fullUrlSections.Length)
            {
                //if the current part of the url match this part of the fullUrl then the result will be true for now
                if (fullUrlSections[currentFileUrlIndex].Equals(urlSection, StringComparison.OrdinalIgnoreCase))
                {
                    result = true;
                    currentFileUrlIndex++;
                    break;
                }

                //this is for handling the situations that the others parts of the url does not match the rest of the fullUrl simultaneously
                if (result)
                {
                    result = false;
                    breakTheLoop = true;
                    break;
                }

                currentFileUrlIndex++;
            }

            if (breakTheLoop)
                break;
        }

        return result;
    }
}