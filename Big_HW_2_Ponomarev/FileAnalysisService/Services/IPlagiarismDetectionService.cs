namespace FileAnalysisService.Services;

public class PlagiarismResult
{
    public bool IsFullPlagiarism { get; set; }
    public Guid? OriginalFileId { get; set; } 
    public string? OriginalFileHash { get; set; }
}

public interface IPlagiarismDetectionService
{
    Task<PlagiarismResult> DetectPlagiarismAsync(Guid fileId, string fileHash, Stream fileContentStream);
}