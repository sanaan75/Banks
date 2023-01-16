using Entities.Journals;

namespace Web.Models.Articles
{
    public class RecordModel
    {
        public string Category { get; set; }
        public double? IF { get; set; }
        public double? AIF { get; set; }
        public double? MIF { get; set; }
        public string Publisher { get; set; }
        public int Year { get; set; }
        public string Index { get; set; }
        public string Issn { get; set; }
        public string EIssn { get; set; }
        public JournalQRank? QRank { get; set; }
        public string JournalType { get; set; }
    }
}