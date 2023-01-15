namespace UseCases.ResultModels
{
    public class RecordModel
    {
        public int Year { get; set; }
        public string Index { get; set; }
        public string Type { get; set; }
        public string Q { get; set; }
        public decimal? IF { get; set; }
        public string Category { get; set; }
        public decimal? Mif { get; set; }
        public decimal? Aif { get; set; }
        public JournalModel Journal { get; set; }
    }
}