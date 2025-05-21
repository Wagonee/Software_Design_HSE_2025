namespace FileAnalysisService.Services;

public class WordCloudGenerationResult
{
    public bool Success { get; set; }
    public string? ImageLocation { get; set; } 
    public string? ErrorMessage { get; set; }
}

public interface IWordCloudGenerationService
{
    Task<WordCloudGenerationResult> GenerateAndStoreWordCloudAsync(Guid fileId, Stream textStream);
}