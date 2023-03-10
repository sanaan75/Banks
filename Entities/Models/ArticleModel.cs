using Entities.Journals;

namespace Entities.Models;

public class ArticleModel
{
    public string Title { get; set; }
    public string URL { get; set; }
    public string Volume { get; set; }
    public List<string> Authors { get; set; }
    public DateTime? DateIndexed { get; set; }
    public string DateCreated { get; set; }
    public string Language { get; set; }
    public string Page { get; set; }
    public string Publisher { get; set; }
    public string JournalTitle { get; set; }
    public string ISSN { get; set; }
    public string Issue { get; set; }
    public string Location { get; set; }
    public List<string> Categories { get; set; }
    public decimal? IF { get; set; }
    public JournalIndex? Index { get; set; }
    public decimal? Mif { get; set; }
    public JournalQRank? QRank { get; set; }
}