using System.Collections.Generic;
using System.Linq;
using Entities;
using Framework;
using UseCases.ResultModels;

namespace UseCases.Journals
{
    public class FindJournal : IFindJournal
    {
        private readonly IUnitOfWork _unitOfWork;

        public FindJournal(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<RecordModel> Respond(IFindJournal.Request request)
        {
            var query = _unitOfWork.JournalRecords.GetAll();
            query = query.Where(i => i.Year < request.Year);

            if (request.Category != null)
                query = query.Where(j => j.Category == request.Category.ToUpper());

            if (request.Title != null)
                query = query.Where(k => k.Journal.Title.Contains(request.Title));

            if (request.Issn != null)
                query = query.Where(k => k.Journal.Issn == request.Issn);

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
                        URL = record.Journal.WebSite,
                        Publisher = record.Journal.Publisher,
                        Country = record.Journal.Country
                    }
                })
                .ToList();

            return new(recordsList.Where(a => a.Year == recordsList.Max(x => x.Year)));
        }
    }
}