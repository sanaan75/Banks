using Entities.Journals;

namespace UseCases.Journals;

public interface IAddJournal
{
    public Journal Responce(Request request);

    public class Request
    {
        public string Title { get; set; }
        public string Issn { get; set; }
        public string EIssn { get; set; }
        public string WebSite { get; set; }
        public string Publisher { get; set; }
        public string Country { get; set; }
    }
}