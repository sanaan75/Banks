using Entities.Journals;
using Entities.Utilities;
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
        return _db.Set<Journal>().Add(new Journal
        {
            Title = request.Title,
            NormalizedTitle = request.Title.VacuumString(),
            Issn = request.Issn.CleanIssn(),
            EIssn = request.EIssn.CleanIssn(),
            WebSite = request.WebSite,
            Publisher = request.Publisher,
            Country = request.Country
        }).Entity;
    }
}