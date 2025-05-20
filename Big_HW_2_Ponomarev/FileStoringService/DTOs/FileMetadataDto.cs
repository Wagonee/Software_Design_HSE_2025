namespace FileStoringService.DTOs;

public class FileMetadataDto
{
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; }
    public string ContentType { get; set; }
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
    public string Hash { get; set; }
}