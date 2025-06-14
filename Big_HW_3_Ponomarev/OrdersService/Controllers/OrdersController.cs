using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using OrdersService.Data.Entities;
using OrdersService.Outbox;

namespace OrdersService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(OrdersDbContext context, SocketManager socketManager) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromQuery] string userId, [FromQuery] decimal amount)
    {
        if (string.IsNullOrEmpty(userId) || amount <= 0)
        {
            return BadRequest("UserId и amount должны быть указаны корректно.");
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = amount
        };

        var @event = new
        {
            MessageId = Guid.NewGuid(),  
            OrderId = order.Id,
            order.UserId,
            order.Amount
        };

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "OrderCreated",
            Payload = JsonSerializer.Serialize(@event)
        };

        context.Orders.Add(order);
        context.OutboxMessages.Add(outboxMessage);

        await context.SaveChangesAsync();
        var notificationMessage = $"Заказ {order.Id} успешно создан и принят в обработку.";
        await socketManager.SendMessage(userId, notificationMessage);
        
        return AcceptedAtAction(nameof(GetOrderStatus), new { orderId = order.Id }, order);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetOrders(string userId)
    {
        var orders = await context.Orders
            .Where(o => o.UserId == userId)
            .ToListAsync();
        
        return Ok(orders);
    }
    
    [HttpGet("status/{orderId}")]
    public async Task<IActionResult> GetOrderStatus(Guid orderId)
    {
        var order = await context.Orders.FindAsync(orderId);

        if (order == null)
        {
            return NotFound($"Заказ с ID {orderId} не найден.");
        }
        return Ok(new { order.Id, order.Status });
    }
}