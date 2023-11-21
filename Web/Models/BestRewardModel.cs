using Entities.Journals;

namespace Web.Models;

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