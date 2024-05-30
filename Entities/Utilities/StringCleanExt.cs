using System.Text.RegularExpressions;

namespace Entities.Utilities;

public static class StringCleanExt
{
    public const char ArabicYe = 'ي';
    public const char ArabicAlefMaksura = 'ى';
    public const char PersianYe = 'ی';
    public const char ArabicKe = 'ك';
    public const char PersianKe = 'ک';
    public const char ArabicHeh = 'ة';
    public const char PersianHeh = 'ه';
    public const char RightToLeftMark = '\u200F';
    public const char ZeroWidthNonJoiner = '\u200C';

    public static string Clean(this string text) => text.CleanInternal();

    public static string CleanAndPreserveSpaces(this string text) => text.CleanInternal(true);

    private static string CleanInternal(this string text, bool preserveSpaces = false)
    {
        if (text == null)
            return (string)null;
        text = text.ReplaceArabicLetters();
        text = StringCleanExt.ReplaceDirtyCharacters(text);
        text = StringCleanExt.ReplaceBadHalfSpaces(text);
        if (!preserveSpaces)
            text = StringCleanExt.ReplaceMultipleSpacesWithSingleSpace(text);
        text = text.Trim();
        return text;
    }

    private static string ReplaceMultipleSpacesWithSingleSpace(string text) => Regex.Replace(text, "\\s{1,}", " ");

    private static string ReplaceDirtyCharacters(string text) => text.Replace('\u200F', ' ');

    private static string ReplaceBadHalfSpaces(string text) => text.Replace('¬', '\u200C');

    public static string ReplaceArabicLetters(this string text) => text != null
        ? text.Replace('ى', 'ی').Replace('ي', 'ی').Replace('ك', 'ک').Replace('ة', 'ه')
        : string.Empty;

    public static string NormalizeText(this string str) => str.Clean();

    public static string CleanForCompare(this string str) => str.Clean().Vacuum().Replace("ء", string.Empty)
        .Replace("\u200C", string.Empty).Replace("ئ", "ی").Replace("آ", "ا").Replace("آْ", "ا").Replace("ْ", "");

    public static string Vacuum(this string text)
    {
        text = text.Clean();
        text = text.Replace('\u200C', ' ');
        text = text.Replace(" ", string.Empty);
        text = text.Replace("ـ", string.Empty);
        return text;
    }

    public static string ToArabicNumber(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;
        text = text.Replace('۰', '0');
        text = text.Replace('٠', '0');
        text = text.Replace('۱', '1');
        text = text.Replace('۲', '2');
        text = text.Replace('۳', '3');
        text = text.Replace('۴', '4');
        text = text.Replace('٤', '4');
        text = text.Replace('۵', '5');
        text = text.Replace('٥', '5');
        text = text.Replace('۵', '5');
        text = text.Replace('۶', '6');
        text = text.Replace('۷', '7');
        text = text.Replace('۸', '8');
        text = text.Replace('۹', '9');
        return text;
    }

    public static string ToHindiNumber(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;
        text = text.Replace('0', '۰');
        text = text.Replace('1', '۱');
        text = text.Replace('2', '۲');
        text = text.Replace('3', '۳');
        text = text.Replace('4', '۴');
        text = text.Replace('5', '۵');
        text = text.Replace('6', '۶');
        text = text.Replace('7', '۷');
        text = text.Replace('8', '۸');
        text = text.Replace('9', '۹');
        return text;
    }

    // public static string VacuumString(this string input)
    // {
    //     string noWhitespace = Regex.Replace(input, @"\s+", "");
    //     return noWhitespace.Trim().ToLower();
    // }
}