using System.Text;
using FileAnalysisService.Services; 
using Microsoft.Extensions.Logging;
using Moq;

namespace FileAnalysisService.Tests 
{
    public class TextStatisticsServiceTests
    {
        private readonly Mock<ILogger<TextStatisticsService>> _mockLogger;
        private readonly TextStatisticsService _service;

        public TextStatisticsServiceTests()
        {
            _mockLogger = new Mock<ILogger<TextStatisticsService>>();
            _service = new TextStatisticsService(_mockLogger.Object);
        }

        private static MemoryStream CreateStreamFromString(string content = "")
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(content));
        }

        [Theory]
        [InlineData("", 0, 0, 0)] 
        [InlineData("Hello world.", 12, 2, 1)] 
        [InlineData("  Leading and trailing spaces.  ", 32, 4, 1)]
        [InlineData("OneWord", 7, 1, 1)] 
        [InlineData("Two words.", 10, 2, 1)]
        [InlineData("Multiple, words! With; different: separators?", 45, 5, 1)] 
        [InlineData("This is a single line of text with no paragraph breaks.", 55, 11, 1)] 
        [InlineData(" ", 1, 0, 1)] 
        [InlineData("\t", 1, 0, 1)] 
        [InlineData("\n", 1, 0, 1)] 
        [InlineData("Line one\nLine two", 17, 4, 1)]
        [InlineData("   \n\n   ", 8, 0, 2)]
        [InlineData("\n\n", 2, 0, 0)] 
        [InlineData("\r\n\r\n", 4, 0, 0)] 
        [InlineData(", . ; : ! ?", 11, 0, 1)] 
        [InlineData("Привет мир", 10, 2, 1)] 
        public async Task CalculateAsync_VariousContentScenarios_CalculatesCorrectly(
            string content, int expectedChars, int expectedWords, int expectedParagraphs)
        {
            using var stream = CreateStreamFromString(content);

            var result = await _service.CalculateAsync(stream);

            Assert.Equal(expectedChars, result.CharCount);
            Assert.Equal(expectedWords, result.WordCount);
            Assert.Equal(expectedParagraphs, result.ParagraphCount);
        }

        [Theory]
        [InlineData("First paragraph.\n\nSecond paragraph.", 35, 4, 2)] 
        [InlineData("Para 1.\r\n\r\nPara 2.", 18, 4, 2)] 
        [InlineData("Para 1.\r\n\r\nPara 2.\n\nPara 3.", 27, 6, 3)] 
        [InlineData("One\n\nTwo\r\n\r\nThree\n\n\n\nFour", 25, 4, 4)] 
        [InlineData("Start.\n\n\n\nEnd.", 14, 2, 2)] 
        [InlineData("\n\nParagraph after empty.", 24, 3, 1)] 
        [InlineData("Paragraph before empty.\n\n", 25, 3, 1)] 
        public async Task CalculateAsync_MultiParagraphContent_CalculatesCorrectly(
            string content, int expectedChars, int expectedWords, int expectedParagraphs)
        {
            using var stream = CreateStreamFromString(content);

            var result = await _service.CalculateAsync(stream);

            
            Assert.Equal(expectedChars, result.CharCount);
            Assert.Equal(expectedWords, result.WordCount);
            Assert.Equal(expectedParagraphs, result.ParagraphCount);
        }

        [Fact]
        public async Task CalculateAsync_LogsInformation_WhenCalled()
        {
            using var stream = CreateStreamFromString("Test log content.");
            await _service.CalculateAsync(stream);
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Calculating text statistics...")), 
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Statistics calculated:")), 
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}