namespace Entities.Utilities.TextSimilarity;

public interface IMetricStringDistance : IStringDistance
{
    new double Distance(string s1, string s2);
}