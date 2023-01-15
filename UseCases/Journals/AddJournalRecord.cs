using Entities.Journals;
using Framework;
using System;
using System.Linq;

namespace UseCases.Journals
{
    public class AddJournalRecord : IAddJournalRecord
    {
        public static string JournalNotFound => "[Journal] یافت نشد.";
        public static string Duplicated => "[Record] تکراری است";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IActorService _actorService;


        public AddJournalRecord(IUnitOfWork unitOfWork,IActorService actorService)
        {
            _unitOfWork = unitOfWork;
            _actorService = actorService;
        }

        public JournalRecord Respond(IAddJournalRecord.Request request)
        {
            var journal = _unitOfWork.Journals.GetById(request.JournalId);
            Check.NotNull(journal, () => JournalNotFound);

            var category = request.Category.ToUpper();

            var historyAlreadyExists = _unitOfWork.JournalRecords.Find(i => i.JournalId == request.JournalId)
                .FilterByCategory(category)
                .Any(i => i.Year == request.Year);
            
            Check.False(historyAlreadyExists, () => Duplicated);
            return _unitOfWork.JournalRecords.Add(new JournalRecord
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
                CreatorId = _actorService.UserId,
                CreateDate = DateTime.UtcNow
            });
        }
    }
}
