using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileAnalysisService.Models;


public enum AnalysisStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
}

[Table("AnalysisResult")]
public class AnalysisResult
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid FileId { get; set; }
    
    public int ParagraphCount { get; set; }
    public int WordCount { get; set; }
    public int CharCount { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string? PlagiarismScores { get; set; }
    public string? WordCloudImageLocation { get; set; } 
    public AnalysisStatus Status { get; set; } = AnalysisStatus.Pending;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    
    [Required]
    [MaxLength(64)] 
    public string FileContentHash { get; set; }
}