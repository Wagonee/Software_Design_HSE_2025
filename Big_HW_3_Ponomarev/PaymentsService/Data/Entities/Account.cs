using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Data.Entities;

public class Account
{
    [Key]
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}