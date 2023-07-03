using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Entities.Utilities.TextSimilarity;

public class ShingleBased
{
    private const int DEFAULT_K = 3;

    private readonly Regex SPACE_REG = new("\\s+");

    protected ShingleBased(int k)
    {
        if (k <= 0) throw new ArgumentOutOfRangeException(nameof(k), "k should be positive!");

        _k = k;
    }

    protected ShingleBased() : this(DEFAULT_K)
    {
    }

    protected int _k { get; set; }

    public IDictionary<string, int> GetProfile(string s)
    {
        var shingles = new Dictionary<string, int>();

        var string_no_space = SPACE_REG.Replace(s, " ");

        for (var i = 0; i < string_no_space.Length - _k + 1; i++)
        {
            var shingle = string_no_space.Substring(i, _k);

            if (shingles.TryGetValue(shingle, out var old))
                shingles[shingle] = old + 1;
            else
                shingles[shingle] = 1;
        }

        return new ReadOnlyDictionary<string, int>(shingles);
    }
}