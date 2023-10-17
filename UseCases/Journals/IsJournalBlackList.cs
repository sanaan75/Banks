using Entities.Journals;
using UseCases.Interfaces;

namespace UseCases.Journals;

public class IsJournalBlackList : IIsJournalBlackList
{
    private readonly IDb _db;

    public IsJournalBlackList(IDb db, IActorService actorService)
    {
        _db = db;
    }

    public bool Responce(string title)
    {
        return _db.Query<BlackList>()
            .Where(i => i.Journal.Title.Trim().ToLower().Equals(title.Trim().ToLower()))
            .Any(i => i.ToDate == null || i.ToDate < DateTime.UtcNow);
    }
}