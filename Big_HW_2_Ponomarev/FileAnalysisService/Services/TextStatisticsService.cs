using System.Text;

namespace FileAnalysisService.Services;

public class TextStatisticsService : ITextStatisticsService
{
    private readonly ILogger<TextStatisticsService> _logger;

    public TextStatisticsService(ILogger<TextStatisticsService> logger)
    {
        _logger = logger;
    }

    public async Task<TextStatistics> CalculateAsync(Stream textStream)
    {
        _logger.LogInformation("Calculating text statistics...");
        string content;
        using (var reader = new StreamReader(textStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true)) 
        {
            content = await reader.ReadToEndAsync();
        }
        textStream.Position = 0; 
        int charCount = content.Length;
        string[] words = content.Split(new char[] { ' ', '\r', '\n', '\t', '.', ',', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        int wordCount = words.Length;
        string[] paragraphs = content.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
        int paragraphCount = paragraphs.Length;
        if (paragraphCount == 0 && !string.IsNullOrWhiteSpace(content))
        {
            paragraphCount = 1;
        }


        _logger.LogInformation("Statistics calculated: Chars={CharCount}, Words={WordCount}, Paragraphs={ParagraphCount}", charCount, wordCount, paragraphCount);
        return new TextStatistics
        {
            CharCount = charCount,
            WordCount = wordCount,
            ParagraphCount = paragraphCount
        };
    }
}