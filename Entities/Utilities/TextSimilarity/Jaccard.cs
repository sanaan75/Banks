namespace Entities.Utilities.TextSimilarity;

public class Jaccard : ShingleBased, IMetricStringDistance, INormalizedStringDistance, INormalizedStringSimilarity
{
    public Jaccard(int k) : base(k)
    {
    }

    public Jaccard()
    {
    }

    public double Distance(string s1, string s2)
    {
        return 1.0 - Similarity(s1, s2);
    }

    public double Similarity(string s1, string s2)
    {
        if (s1 == null) throw new ArgumentNullException(nameof(s1));

        if (s2 == null) throw new ArgumentNullException(nameof(s2));

        if (s1.Equals(s2)) return 1;

        var profile1 = GetProfile(s1);
        var profile2 = GetProfile(s2);

        var union = new HashSet<string>();
        union.UnionWith(profile1.Keys);
        union.UnionWith(profile2.Keys);

        var inter = profile1.Keys.Count + profile2.Keys.Count
                    - union.Count;

        return 1.0 * inter / union.Count;
    }

    public bool IsSimilar(string s1, string s2, int similarityPercent)
    {
        if (s2 == null) throw new ArgumentNullException(nameof(s2));

        if (s1.Equals(s2)) return true;

        var profile1 = GetProfile(s1);
        var profile2 = GetProfile(s2);

        var union = new HashSet<string>();
        union.UnionWith(profile1.Keys);
        union.UnionWith(profile2.Keys);

        var inter = profile1.Keys.Count + profile2.Keys.Count - union.Count;

        var similarity = 1.0 * inter / union.Count * 100;
        if (similarity < similarityPercent)
            return false;

        return true;
    }
}