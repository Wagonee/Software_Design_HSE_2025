namespace FileAnalysisService.Services;

public class TextStatistics
{
    public int ParagraphCount { get; set; }
    public int WordCount { get; set; }
    public int CharCount { get; set; }
}

public interface ITextStatisticsService
{
    Task<TextStatistics> CalculateAsync(Stream textStream);
}