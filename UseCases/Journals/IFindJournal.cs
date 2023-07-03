using UseCases.ResultModels;

namespace UseCases.Journals;

public interface IFindJournal
{
    public List<RecordModel> Respond(Request request);

    public class Request
    {
        public int Year { get; set; }
        public string Title { get; set; }
        public string Issn { get; set; }
        public string Category { get; set; }
    }
}