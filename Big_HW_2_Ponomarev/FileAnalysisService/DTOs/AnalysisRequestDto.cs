using System;
using System.ComponentModel.DataAnnotations;

namespace FileAnalysisService.DTOs;

public class AnalysisRequestDto
{
    [Required]
    public Guid FileId { get; set; }

    [Required]
    public string FileHash { get; set; }
    public bool ForceReanalyze { get; set; } = false;
}