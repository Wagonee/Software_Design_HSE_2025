using FileAnalysisService.Data;
using FileAnalysisService.Models;
using Microsoft.EntityFrameworkCore;

namespace FileAnalysisService.Services
{
    public class PlagiarismDetectionService : IPlagiarismDetectionService
    {
        private readonly FileAnalysisDbContext _analysisDbContext;
        private readonly Clients.IFileStoringServiceClient _fileStoringServiceClient; 
        private readonly ILogger<PlagiarismDetectionService> _logger;

        public PlagiarismDetectionService(
            FileAnalysisDbContext analysisDbContext,
            Clients.IFileStoringServiceClient fileStoringServiceClient,
            ILogger<PlagiarismDetectionService> logger)
        {
            _analysisDbContext = analysisDbContext;
            _fileStoringServiceClient = fileStoringServiceClient; 
            _logger = logger;
        }

        public async Task<PlagiarismResult> DetectPlagiarismAsync(Guid currentFileId, string currentFileHash, Stream fileContentStream)
        {
            _logger.LogInformation("Detecting plagiarism for FileId: {CurrentFileId} with Hash: {CurrentFileHash}", currentFileId, currentFileHash);
            
            var existingAnalysisWithSameHash = await _analysisDbContext.AnalysisResults
                .AsNoTracking()
                .FirstOrDefaultAsync(ar => ar.FileContentHash == currentFileHash && ar.FileId != currentFileId && ar.Status == AnalysisStatus.Completed);

            if (existingAnalysisWithSameHash != null)
            {
                _logger.LogInformation("Full plagiarism detected for FileId: {CurrentFileId}. Original FileId with same hash: {OriginalFileId}", currentFileId, existingAnalysisWithSameHash.FileId);
                return new PlagiarismResult
                {
                    IsFullPlagiarism = true,
                    OriginalFileId = existingAnalysisWithSameHash.FileId,
                    OriginalFileHash = currentFileHash
                };
            }

            _logger.LogInformation("No 100% plagiarism detected based on hash comparison with previously analyzed files for FileId: {CurrentFileId}", currentFileId);
            return new PlagiarismResult { IsFullPlagiarism = false };
        }
    }
}