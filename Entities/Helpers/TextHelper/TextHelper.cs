using System;
using System.Collections.Generic;

namespace Entities.Helpers.TextHelper
{
    public static class TextHelper
    {
        public static IList<string> GetTokens(this string str)
        {
            return
                string.IsNullOrWhiteSpace(str) ?
                    Array.Empty<string>() :
                    str.Split(new[] { ' ', '\n', '\r', '\t', '-', ',', '،', '.', '?' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
