namespace Web.Models;

public class RecordModel
{
    public int Id { get; set; }
    public string JournalTitle { get; set; }
    public string Category { get; set; }
    public int Year { get; set; }
    public string Index { get; set; }
    public string Type { get; set; }
    public string QRank { get; set; }
    public decimal? If { get; set; }
    public decimal? Mif { get; set; }
    public decimal? Aif { get; set; }
}