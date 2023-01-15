using Entities.Journals;
using Framework;
using Services;
using AppContext = Persistence.AppContext;

namespace UseCases.Journals
{
    public class JournalRepository : BaseRepository<AppContext, Journal>, IJournalRepository
    {
        public JournalRepository(AppContext appContext)
            : base(appContext)
        {
        }
    }
}