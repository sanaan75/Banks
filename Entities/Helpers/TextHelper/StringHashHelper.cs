using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Entities.Helpers.TextHelper;

public static class StringHashHelper
{
    private static byte[] GetUtf8Bytes(string str)
    {
        return new UTF8Encoding().GetBytes(str);
    }

    public static byte[] CalculateStringMD5Hash(string str)
    {
        var utf8Bytes = GetUtf8Bytes(str);
        using (var cryptoServiceProvider = new MD5CryptoServiceProvider())
        {
            return cryptoServiceProvider.ComputeHash(utf8Bytes);
        }
    }

    public static byte[] CalculateStringSha1Hash(string str)
    {
        return new SHA1Managed().ComputeHash(GetUtf8Bytes(str));
    }

    public static string CalculateStringSha1HashString(string str)
    {
        return Convert.ToBase64String(CalculateStringSha1Hash(str));
    }

    public static byte[] CalculateStringSha512Hash(string str)
    {
        return new SHA512Managed().ComputeHash(GetUtf8Bytes(str));
    }

    public static string CalculateStringSha512HashString(string str)
    {
        return Convert.ToBase64String(CalculateStringSha512Hash(str));
    }
}

public static class StringGlossaryExt
{
    public static string ApplyGlossary(this string str)
    {
        foreach (Match match in new Regex("\\[[^\\]]+\\]").Matches(str))
        {
            var key = match.Value.Trim('[', ']');
            str = str.Replace(match.Value, GlossaryExt.Get(key));
        }

        return str;
    }
}