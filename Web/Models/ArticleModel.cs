using Entities.Journals;
using Entities.Utilities;
using UseCases.Journals;

namespace Web.Models
{
    public class ArticleModel
    {
        public string Title { get; set; }
        public string URL { get; set; }
        public List<string> Authors { get; set; }
        public DateTime? DateIndexed { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Language { get; set; }

        public string? JournalTitle { get; set; }
        public string ISSN { get; set; }

        public List<string> Categories { get; set; }

        public JournalIndex? Index { get; set; }
        public string IndexCaption => Index.GetCaption();

        public JournalQRank? QRank { get; set; }
        public string QRankCaption => QRank.GetCaption();
        public decimal? IF { get; set; }
        public decimal? Mif { get; set; }
        public decimal? Aif { get; set; }

        public string Page { get; set; }
        public string Volume { get; set; }
        public string Publisher { get; set; }
        public string Issue { get; set; }
        public string Location { get; set; }

        public IGetBestJournalInfo.BestInfoModel BestInfo { get; set; }
    }
}