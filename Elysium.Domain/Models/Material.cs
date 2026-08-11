namespace Elysium.Domain.Models;

public class Material
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public string Name { get; set; } = default!;
    public string StoredPath { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long SizeBytes { get; set; }
    public DateTime UploadedAt { get; set; }

    public Session Session { get; set; } = default!;
}
