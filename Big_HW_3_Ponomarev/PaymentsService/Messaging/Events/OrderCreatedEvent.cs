namespace PaymentsService.Messaging.Events;

public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}