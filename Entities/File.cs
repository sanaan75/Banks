namespace Entities;

public class File
{
    public int Id { get; set; }
    public string? Extension { get; set; }
    public byte[]? Data { get; set; }
    public long Size { get; set; }
    public byte[]? HashCode { get; set; }
    public DateTime? UploadDate { get; set; }
}