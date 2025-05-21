using FileAnalysisService.Models;

namespace FileAnalysisService.Services;

public interface IFileAnalysisOrchestratorService
{
    Task<AnalysisResult> AnalyzeFileAsync(Guid fileId, string fileHash);
}