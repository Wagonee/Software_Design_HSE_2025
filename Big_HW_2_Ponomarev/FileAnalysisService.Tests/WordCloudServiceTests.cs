using System.Text;
using FileAnalysisService.Clients; 
using FileAnalysisService.Services; 
using Microsoft.Extensions.Logging;
using Moq;


namespace FileAnalysisService.Tests
{
    public class WordCloudGenerationServiceTests
    {
        private readonly Mock<IWordCloudApiClient> _mockWordCloudApiClient;
        private readonly Mock<IFileStorageProvider> _mockLocalStorageProvider; 
        private readonly Mock<ILogger<WordCloudGenerationService>> _mockLogger;
        private readonly WordCloudGenerationService _service;

        public WordCloudGenerationServiceTests()
        {
            _mockWordCloudApiClient = new Mock<IWordCloudApiClient>();
            _mockLocalStorageProvider = new Mock<IFileStorageProvider>();
            _mockLogger = new Mock<ILogger<WordCloudGenerationService>>(); 
            _service = new WordCloudGenerationService( 
                _mockWordCloudApiClient.Object,
                _mockLocalStorageProvider.Object, 
                _mockLogger.Object);
        }

        private static MemoryStream CreateStreamFromString(string content)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(content ?? ""));
        }

        [Fact]
        public async Task GenerateAndStoreWordCloudAsync_ErrorReadingStream_ReturnsFailureResult()
        {
   
            var fileId = Guid.NewGuid();
            var mockStream = new Mock<Stream>();
            mockStream.Setup(s => s.ReadAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                      .ThrowsAsync(new IOException("Stream read error"));
            var faultyStream = new MemoryStream();
            await faultyStream.DisposeAsync(); 

            var result = await _service.GenerateAndStoreWordCloudAsync(fileId, faultyStream);

            Assert.False(result.Success);
            Assert.Equal("Error reading text content for word cloud.", result.ErrorMessage);
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Error reading text stream for FileId: {fileId} for word cloud generation.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Theory]
        [InlineData(null)] 
        [InlineData("")]
        [InlineData("   ")]
        public async Task GenerateAndStoreWordCloudAsync_EmptyOrWhitespaceTextContent_ReturnsFailureResult(string textContent)
        {
            var fileId = Guid.NewGuid();
            using var stream = CreateStreamFromString(textContent);

            
            var result = await _service.GenerateAndStoreWordCloudAsync(fileId, stream);

            
            Assert.False(result.Success);
            Assert.Equal("Text content is empty.", result.ErrorMessage);
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Cannot generate word cloud for FileId: {fileId} because text content is empty.")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GenerateAndStoreWordCloudAsync_ApiClientReturnsNullStream_ReturnsFailureResult()
        {

            var fileId = Guid.NewGuid();
            var textContent = "Valid text";
            using var stream = CreateStreamFromString(textContent);

            _mockWordCloudApiClient
                .Setup(client => client.GenerateWordCloudAsync(textContent, It.IsAny<WordCloudParameters>()))
                .ReturnsAsync((Stream?)null);


            var result = await _service.GenerateAndStoreWordCloudAsync(fileId, stream);


            Assert.False(result.Success);
            Assert.Equal("Failed to generate word cloud image from API (empty or null response).", result.ErrorMessage);
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"WordCloudApiClient returned no image or empty stream for FileId: {fileId}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
        
        [Fact]
        public async Task GenerateAndStoreWordCloudAsync_ApiClientReturnsEmptyStream_ReturnsFailureResult()
        {
       
            var fileId = Guid.NewGuid();
            var textContent = "Valid text";
            using var textStream = CreateStreamFromString(textContent);
            using var emptyImageStream = new MemoryStream(); 

            _mockWordCloudApiClient
                .Setup(client => client.GenerateWordCloudAsync(textContent, It.IsAny<WordCloudParameters>()))
                .ReturnsAsync(emptyImageStream);


            var result = await _service.GenerateAndStoreWordCloudAsync(fileId, textStream);

    
            Assert.False(result.Success);
            Assert.Equal("Failed to generate word cloud image from API (empty or null response).", result.ErrorMessage);
             _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"WordCloudApiClient returned no image or empty stream for FileId: {fileId}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GenerateAndStoreWordCloudAsync_SuccessfulGenerationAndStorage_ReturnsSuccessResult()
        {

            var fileId = Guid.NewGuid();
            var textContent = "This is valid text for the word cloud.";
            using var textStream = CreateStreamFromString(textContent);
            
            var imageBytes = Encoding.UTF8.GetBytes("fake image data");
            var imageStream = new MemoryStream(imageBytes); 
            
            var expectedStoredFileName = $"{fileId}_wordcloud.png";
            var expectedStoredPath = $"/storage/{expectedStoredFileName}";

            _mockWordCloudApiClient
                .Setup(client => client.GenerateWordCloudAsync(
                    textContent, 
                    It.Is<WordCloudParameters>(p => p.Language == "ru" && p.Format == "png")))
                .ReturnsAsync(imageStream);

            _mockLocalStorageProvider
                .Setup(provider => provider.SaveFileAsync(It.IsAny<Stream>(), expectedStoredFileName, "image/png"))
                .ReturnsAsync(expectedStoredPath);

   
            var result = await _service.GenerateAndStoreWordCloudAsync(fileId, textStream);

     
            Assert.True(result.Success);
            Assert.Equal(expectedStoredPath, result.ImageLocation);
            Assert.Null(result.ErrorMessage);
            _mockLocalStorageProvider.Verify(provider => provider.SaveFileAsync(It.IsAny<Stream>(), expectedStoredFileName, "image/png"), Times.Once);
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Word cloud image for FileId: {fileId} stored locally as {expectedStoredPath}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GenerateAndStoreWordCloudAsync_ApiClientThrowsException_ReturnsFailureResult()
        {

            var fileId = Guid.NewGuid();
            var textContent = "Text causing API error";
            using var stream = CreateStreamFromString(textContent);
            var apiException = new HttpRequestException("API error");

            _mockWordCloudApiClient
                .Setup(client => client.GenerateWordCloudAsync(textContent, It.IsAny<WordCloudParameters>()))
                .ThrowsAsync(apiException);


            var result = await _service.GenerateAndStoreWordCloudAsync(fileId, stream);


            Assert.False(result.Success);
            Assert.Equal($"An unexpected error occurred during word cloud processing: {apiException.Message}", result.ErrorMessage);
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Error generating or storing word cloud for FileId: {fileId}")),
                    apiException,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GenerateAndStoreWordCloudAsync_StorageProviderThrowsException_ReturnsFailureResult()
        {
     
            var fileId = Guid.NewGuid();
            var textContent = "Text causing storage error";
            using var textStream = CreateStreamFromString(textContent);
            
            var imageBytes = Encoding.UTF8.GetBytes("fake image data");
            var imageStream = new MemoryStream(imageBytes);
            
            var storageException = new IOException("Storage error");

             _mockWordCloudApiClient
                .Setup(client => client.GenerateWordCloudAsync(
                    textContent, 
                    It.Is<WordCloudParameters>(p => p.Language == "ru" && p.Format == "png")))
                .ReturnsAsync(imageStream);

            _mockLocalStorageProvider
                .Setup(provider => provider.SaveFileAsync(It.IsAny<Stream>(), $"{fileId}_wordcloud.png", "image/png"))
                .ThrowsAsync(storageException);

            var result = await _service.GenerateAndStoreWordCloudAsync(fileId, textStream);

            Assert.False(result.Success);
            Assert.Equal($"An unexpected error occurred during word cloud processing: {storageException.Message}", result.ErrorMessage);
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Error generating or storing word cloud for FileId: {fileId}")),
                    storageException,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}