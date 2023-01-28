using Entities.Journals;

namespace Entities.Models
{
    public class CategoryModel
    {
        public string Title { get; set; }
        public JournalQRank? Rank { get; set; }
    }
}
