using Entities.Journals;

namespace Web.Models.Journals
{
    public class CategoryModel
    {
        public string Title { get; set; }
        public JournalQRank? Rank { get; set; }
    }
}
