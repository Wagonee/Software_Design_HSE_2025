using FileAnalysisService.Clients;
using System.Text; 

namespace FileAnalysisService.Services
{
    public class WordCloudGenerationService : IWordCloudGenerationService
    {
        private readonly IWordCloudApiClient _wordCloudApiClient;
        private readonly IFileStorageProvider _localStorageProvider;
        private readonly ILogger<WordCloudGenerationService> _logger;

        public WordCloudGenerationService(
            IWordCloudApiClient wordCloudApiClient,
            IFileStorageProvider localStorageProvider,
            ILogger<WordCloudGenerationService> logger)
        {
            _wordCloudApiClient = wordCloudApiClient;
            _localStorageProvider = localStorageProvider;
            _logger = logger;
        }

        public async Task<WordCloudGenerationResult> GenerateAndStoreWordCloudAsync(Guid fileId, Stream textStream)
        {
            _logger.LogInformation("Generating word cloud for FileId: {FileId}", fileId);
            string textContent;
            try
            {
                using (var reader = new StreamReader(textStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true))
                {
                    textContent = await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading text stream for FileId: {FileId} for word cloud generation.", fileId);
                return new WordCloudGenerationResult { Success = false, ErrorMessage = "Error reading text content for word cloud." };
            }

            if (string.IsNullOrWhiteSpace(textContent))
            {
                _logger.LogWarning("Cannot generate word cloud for FileId: {FileId} because text content is empty.", fileId);
                return new WordCloudGenerationResult { Success = false, ErrorMessage = "Text content is empty." };
            }

            try
            {
                _logger.LogInformation("Calling WordCloudApiClient for FileId: {FileId}", fileId);
                var wcParams = new WordCloudParameters { Language = "ru", Format = "png" }; 
                Stream? imageStream = await _wordCloudApiClient.GenerateWordCloudAsync(textContent, wcParams);

                if (imageStream == null || imageStream.Length == 0)
                {
                    _logger.LogWarning("WordCloudApiClient returned no image or empty stream for FileId: {FileId}", fileId);
                    return new WordCloudGenerationResult { Success = false, ErrorMessage = "Failed to generate word cloud image from API (empty or null response)." };
                }

                string storedImageName;
                using (imageStream)
                {
                    string originalImageFileNameForStorage = $"{fileId}_wordcloud.png"; 
                    _logger.LogDebug("Attempting to save word cloud image via _localStorageProvider for FileId: {FileId}", fileId);
                    storedImageName = await _localStorageProvider.SaveFileAsync(imageStream, originalImageFileNameForStorage, "image/png");
                }

                _logger.LogInformation("Word cloud image for FileId: {FileId} stored locally as {StoredImageName}", fileId, storedImageName);
                return new WordCloudGenerationResult { Success = true, ImageLocation = storedImageName };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating or storing word cloud for FileId: {FileId}", fileId);
                return new WordCloudGenerationResult { Success = false, ErrorMessage = $"An unexpected error occurred during word cloud processing: {ex.Message}" };
            }
        }
    }
}