using Entities.Journals;
using Entities.Models;
using Entities.Utilities;
using UseCases.Interfaces;
using UseCases.ResultModels;

namespace UseCases.Journals;

public class FindJournal : IFindJournal
{
    private readonly IDb _db;

    public FindJournal(IDb db)
    {
        _db = db;
    }

    public List<RecordModel> Respond(IFindJournal.Request request)
    {
        var query = _db.Query<JournalRecord>();
        query = query.Where(i => i.Year < request.Year);

        if (string.IsNullOrEmpty(request.Category) == false)
            query = query.Where(j => j.Category.Trim().ToLower() == request.Category.Trim().ToLower());

        if (string.IsNullOrEmpty(request.Title) == false)
            query = query.Where(k => k.Journal.Title.Trim().ToLower().Contains(request.Title.Trim().ToLower()));

        if (string.IsNullOrEmpty(request.Issn) == false)
            query = query.FilterByIssn(request.Issn);

        var recordsList = query.Select(record => new RecordModel
            {
                Year = record.Year,
                Index = record.Index.GetCaption(),
                Type = record.Type != null ? record.Type.GetCaption() : string.Empty,
                Q = record.QRank != null ? record.QRank.GetCaption() : string.Empty,
                Category = record.Category,
                IF = record.If,
                Mif = record.Mif,
                Aif = record.Aif,
                Journal = new JournalModel
                {
                    Title = record.Journal.Title,
                    ISSN = record.Journal.Issn,
                    WebSite = record.Journal.WebSite,
                    Publisher = record.Journal.Publisher,
                    Country = record.Journal.Country
                }
            })
            .ToList();

        return new List<RecordModel>(recordsList.Where(a => a.Year == recordsList.Max(x => x.Year)));
    }
}