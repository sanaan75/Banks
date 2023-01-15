namespace Entities;

public class FileContent : IEntity
{
    public int Id { get; set; }
    public byte[]? Data { get; set; }
    public byte[]? HashCode { get; set; }
}