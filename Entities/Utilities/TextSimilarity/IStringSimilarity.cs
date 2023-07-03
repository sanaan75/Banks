namespace Entities.Utilities.TextSimilarity;

public interface IStringSimilarity
{
    double Similarity(string s1, string s2);
}