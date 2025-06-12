using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Inbox;

public class InboxMessage
{
    [Key]
    public Guid Id { get; set; }
    public DateTime ProcessedAt { get; set; } 
}