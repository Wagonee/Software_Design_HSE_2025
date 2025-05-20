using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStoringService.Models
{
    [Table("StoredFiles")]
    public class StoredFile
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string OriginalFileName { get; set; }

        [Required]
        [MaxLength(100)]
        public string ContentType { get; set; }

        [Required]
        [MaxLength(64)]
        public string Hash { get; set; }

        [Required]
        [MaxLength(1024)]
        public string StoredFileName { get; set; }

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; }
    }
}