using Entities.Journals;

namespace Web.Models
{
    public class ISCRecordList
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public string? ISSN { get; set; }
        public JournalType? Type { get; set; }
        public JournalIscClass? IscClass { get; set; }
        public JournalQRank? QRank { get; set; }
        public decimal? If { get; set; }
    }
}