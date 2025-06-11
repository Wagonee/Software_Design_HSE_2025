using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using OrdersService.Data.Entities;
using OrdersService.Outbox;

namespace OrdersService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(OrdersDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromQuery] string userId, [FromQuery] decimal amount)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = amount
        };

        var @event = new
        {
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
        return Ok(order);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetOrders(string userId)
    {
        var orders = await context.Orders.Where(o => o.UserId == userId).ToListAsync();
        return Ok(orders);
    }
}