namespace Web.Models;

public class DataItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ISSN { get; set; }
    public string? Website { get; set; }
    public string? Publisher { get; set; }
    public string? Country { get; set; }
}