using Entities.Journals;

namespace Web.Models.Journals
{
    public class JournalRecordModel
    {
        public string JournalName { get; set; }
        public string ISSN { get; set; }
        public string EISSN { get; set; }
        public int Year { get; set; }
        public JournalIndex Index { get; set; }
        public string Category { get; set; }
        public string Country { get; set; }
        public string Publisher { get; set; }
        public decimal? IF { get; set; }
        public JournalQRank? Rank { get; set; }
    }
}
