using Entities.Journals;

namespace Web.Models
{
    public class CategoryModel
    {
        public string Title { get; set; }
        public JournalQRank? Rank { get; set; }
    }
}