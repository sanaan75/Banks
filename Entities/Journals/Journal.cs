using Entities.Users;

namespace Entities.Journals;

public class Journal : IEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Issn { get; set; }
    public string? EIssn { get; set; }
    public string? WebSite { get; set; }
    public string? Publisher { get; set; }
    public string? Country { get; set; }
    public DateTime? CreateDate { get; set; }
    public int? CreatorId { get; set; }
    public User Creator { get; set; }
    public DateTime? LastUpdateDate { get; set; }
    public int? LastUpdaterId { get; set; }
    public User LastUpdater { get; set; }
    public ICollection<JournalRecord> Records { get; set; }
}