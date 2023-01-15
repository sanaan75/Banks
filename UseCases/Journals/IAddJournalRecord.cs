using Entities.Journals;

namespace UseCases.Journals
{
    public interface IAddJournalRecord
    {
        public JournalRecord Respond(Request request);

        public class Request
        {
            public int JournalId { get; set; }
            public int Year { get; set; }
            public JournalIndex Index { get; set; }
            public JournalType? JournalType { get; set; }
            public JournalValue? Value { get; set; }
            public JournalIscClass? IscClass { get; set; }
            public JournalQRank? QRank { get; set; }
            public decimal? If { get; set; }
            public string Category { get; set; }
            public decimal? Mif { get; set; }
            public decimal? Aif { get; set; }
        }
    }
}
