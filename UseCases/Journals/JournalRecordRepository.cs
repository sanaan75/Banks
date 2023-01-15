using Entities.Journals;
using Framework;
using Services;
using AppContext = Persistence.AppContext;

namespace UseCases.Journals
{
    public class JournalRecordRepository : BaseRepository<AppContext, JournalRecord>, IJournalRecordRepository
    {
        public JournalRecordRepository(AppContext appContext)
            : base(appContext)
        {
        }
    }
}