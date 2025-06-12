namespace OrdersService.Messaging.Events;

public class PaymentResultEvent
{
    public Guid OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
}