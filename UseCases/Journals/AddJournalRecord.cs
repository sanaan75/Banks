using Entities.Journals;
using Entities.Utilities;
using UseCases.Interfaces;

namespace UseCases.Journals;

public class AddJournalRecord : IAddJournalRecord
{
    private readonly IActorService _actorService;

    private readonly IDb _db;


    public AddJournalRecord(IDb db, IActorService actorService)
    {
        _db = db;
        _actorService = actorService;
    }

    public static string JournalNotFound => "[Journal] یافت نشد.";
    public static string Duplicated => "[Record] تکراری است";

    public JournalRecord Respond(IAddJournalRecord.Request request)
    {
        var journal = _db.Query<Journal>().GetById(request.JournalId);
        Check.NotNull(journal, () => JournalNotFound);

        var category = request.Category.ToUpper();

        var historyAlreadyExists = _db.Query<JournalRecord>().FilterByJournal(request.JournalId)
            .FilterByCategory(category)
            .Any(i => i.Year == request.Year);

        Check.False(historyAlreadyExists, () => Duplicated);
        return _db.Set<JournalRecord>().Add(new JournalRecord
        {
            JournalId = request.JournalId,
            Year = request.Year,
            Index = request.Index,
            Type = request.JournalType,
            Value = request.Value,
            IscClass = request.IscClass,
            QRank = request.QRank,
            If = request.If,
            Category = category,
            Mif = request.Mif,
            Aif = request.Aif,
            Source = request.Source
        }).Entity;
    }
}