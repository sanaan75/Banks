namespace Web.Models.Records
{
    public class AddRecordModel
    {
        public int JournalId { get; set; }
        public string Category { get; set; }
        public int Year { get; set; }
        public int? Index { get; set; }
        public int? Type { get; set; }
        public int? QRank { get; set; }
        public decimal? IF { get; set; }
        public decimal? MIF { get; set; }
        public decimal? AIF { get; set; }
    }
}