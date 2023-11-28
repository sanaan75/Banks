using Entities.Journals;

namespace UseCases.Journals;

public interface IGetBestJournalInfo
{
    public BestInfoModel Respond(IQueryable<JournalRecord> items);

    public class BestInfoModel
    {
        public BestInfoDetailModel BestIf { get; set; }
        public BestInfoDetailModel BestRank { get; set; }
        public BestInfoDetailModel BestIndex { get; set; }
        public BestInfoDetailModel BestReward { get; set; }
    }

    public class BestInfoDetailModel
    {
        public decimal? If { get; set; }
        public decimal? Mif { get; set; }
        public string Category { get; set; }
        public string Rank { get; set; }
        public string Index { get; set; }
    }

    public class BestRewardModel
    {
        public decimal? If { get; set; }
        public decimal? Mif { get; set; }
        public string Category { get; set; }
        public JournalQRank? Rank { get; set; }
        public JournalIndex? Index { get; set; }
        public int Value { get; set; }
        public int Year { get; set; }
    }
}