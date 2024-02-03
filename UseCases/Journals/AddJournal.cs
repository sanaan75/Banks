using Entities.Journals;
using UseCases.Interfaces;

namespace UseCases.Journals;

public class AddJournal : IAddJournal
{
    private readonly IDb _db;

    public AddJournal(IDb db)
    {
        _db = db;
    }

    public Journal Responce(IAddJournal.Request request)
    {
        var journalExist = _db
            .Query<Journal>().Any(i => i.Title.Trim().ToLower() == request.Title.Trim().ToLower());

        Check.False(journalExist, () => "مجله تکراری است");

        return _db.Set<Journal>().Add(new Journal
        {
            Title = request.Title,
            Issn = request.Issn,
            EIssn = request.EIssn,
            WebSite = request.WebSite,
            Publisher = request.Publisher,
            Country = request.Country
        }).Entity;
    }
}