using Entities.Journals;

namespace Web.Models
{
    public class JournalRecordList
    {
        public string Category { get; set; }
        public int Year { get; set; }
        public JournalIndex? Index { get; set; }
        public JournalType? Type { get; set; }
        public JournalValue? Value { get; set; }
        public JournalIscClass? IscClass { get; set; }
        public JournalQRank? QRank { get; set; }
        public decimal? If { get; set; }
        public decimal? Mif { get; set; }
        public decimal? Aif { get; set; }
    }
}