using Entities.Journals;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.Journals
{
    public class AddJournal : IAddJournal
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActorService _actorService;

        public AddJournal(IUnitOfWork unitOfWork, IActorService actorService)
        {
            _unitOfWork = unitOfWork;
            _actorService = actorService;
        }

        public Journal Responce(IAddJournal.Request request)
        {
            var journalExist = _unitOfWork.Journals
                .Find(i => i.Title.Trim().ToLower() == request.Title.Trim().ToLower()).Any();

            Check.False(journalExist, () => "مجله تکراری است");

            return _unitOfWork.Journals.Add(new Journal
            {
                Title = request.Title,
                Issn = request.Issn,
                EIssn = request.EIssn,
                WebSite = request.WebSite,
                Publisher = request.Publisher,
                Country = request.Country,
                CreateDate = DateTime.Now,
                CreatorId = _actorService.UserId
            });
        }
    }
}