using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Entities
{
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

                int year = Convert.ToInt32(dateArr[0]);
                int month = Convert.ToInt32(dateArr[1]);
                int day = Convert.ToInt32(dateArr[2]);

                DateTime dt = new DateTime(year, month, day, new PersianCalendar());
                return dt;
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? GetDateTime(string date)
        {
            try
            {
                int year = Convert.ToInt32(date.Substring(0, 4));
                int month = Convert.ToInt32(date.Substring(5, 2));
                int day = Convert.ToInt32(date.Substring(8, 2));

                DateTime dt = new DateTime(year, month, day, new PersianCalendar());
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
                if (text != null)
                {
                    Dictionary<char, char> LettersDictionary = new Dictionary<char, char>
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
                    foreach (var item in text)
                    {
                        text = text.Replace(item, LettersDictionary[item]);
                    }
                }

                return text;
            }
            catch
            {
                return string.Empty;
            }
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
    }
}