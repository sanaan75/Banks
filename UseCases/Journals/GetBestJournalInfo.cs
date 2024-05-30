using Entities.Journals;

namespace UseCases.Journals;

public class GetBestJournalInfo : IGetBestJournalInfo
{
    public IGetBestJournalInfo.BestInfoModel Respond(IQueryable<JournalRecord> items)
    {
        var bestIf = items.OrderByDescending(i => i.Year)
            .ThenByDescending(i => i.If)
            .Select(i => new IGetBestJournalInfo.BestInfoDetailModel
            {
                RankValue = i.QRank,
                If = i.If,
                Mif = i.Mif,
                IndexValue = i.Index,
                Category = i.Category
            }).FirstOrDefault();

        var bestRank = items.OrderByDescending(i => i.Year)
            .ThenBy(i => i.QRank)
            .Where(i => i.QRank != null)
            .Select(i => new IGetBestJournalInfo.BestInfoDetailModel
            {
                RankValue = i.QRank,
                If = i.If,
                Mif = i.Mif,
                IndexValue = i.Index,
                Category = i.Category
            }).FirstOrDefault();

        var bestIndex = items.OrderByDescending(i => i.Year)
            .ThenBy(i => i.Index)
            .Where(i => i.Index != null)
            .Select(i => new IGetBestJournalInfo.BestInfoDetailModel
            {
                RankValue = i.QRank,
                If = i.If,
                Mif = i.Mif,
                IndexValue = i.Index,
                Category = i.Category
            }).FirstOrDefault();

        var list = items
            .OrderByDescending(i => i.Year)
            .ThenBy(i => i.QRank)
            .ThenBy(i => i.Index)
            .Where(i => i.Index != null);

        var bestReward =
            SortForReward(list)
                .OrderBy(m => m.Value)
                .Select(i => new IGetBestJournalInfo.BestInfoDetailModel
                {
                    RankValue = i.Rank,
                    If = i.If,
                    Mif = i.Mif,
                    IndexValue = i.Index,
                    Category = i.Category
                }).FirstOrDefault();

        return new IGetBestJournalInfo.BestInfoModel
            { BestIf = bestIf, BestRank = bestRank, BestIndex = bestIndex, BestReward = bestReward };
    }

    public List<IGetBestJournalInfo.BestRewardModel> SortForReward(IQueryable<JournalRecord> records)
    {
        List<IGetBestJournalInfo.BestRewardModel> list = new List<IGetBestJournalInfo.BestRewardModel>();
        foreach (var record in records)
        {
            var item = new IGetBestJournalInfo.BestRewardModel
            {
                Category = record.Category,
                Index = record.Index,
                Rank = record.QRank,
                If = record.If,
                Mif = record.Mif,
                Year = record.Year
            };
            switch (record.Index)
            {
                case JournalIndex.JCR:
                {
                    item.Value = record.QRank switch
                    {
                        JournalQRank.Q1 => 1,
                        JournalQRank.Q2 => 2,
                        JournalQRank.Q3 => 5,
                        JournalQRank.Q4 => 7,
                        _ => 9
                    };
                    break;
                }
                case JournalIndex.Scopus:
                {
                    item.Value = record.QRank switch
                    {
                        JournalQRank.Q1 => 3,
                        JournalQRank.Q2 => 4,
                        JournalQRank.Q3 => 6,
                        JournalQRank.Q4 => 8,
                        _ => 9
                    };
                    break;
                }
                default:
                {
                    item.Value = 9;
                    break;
                }
            }

            list.Add(item);
        }

        return list;
    }
}