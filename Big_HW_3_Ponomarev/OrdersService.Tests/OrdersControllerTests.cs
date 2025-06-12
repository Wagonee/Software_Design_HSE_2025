using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersService.Controllers;
using OrdersService.Data;
using OrdersService.Data.Entities;

namespace OrdersService.Tests;

public class OrdersControllerTests
{
    private readonly OrdersDbContext _dbContext;
    private readonly OrdersController _controller;

    public OrdersControllerTests()
    {
        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseInMemoryDatabase($"TestDb-{Guid.NewGuid()}")
            .Options;
            
        _dbContext = new OrdersDbContext(options);

        _controller = new OrdersController(_dbContext);
    }

    [Fact]
    public async Task CreateOrder_WithValidData_ShouldCreateOrderAndOutboxMessage()
    {
        var userId = "test-user";
        var amount = 150m;

        var actionResult = await _controller.CreateOrder(userId, amount);

        var result = actionResult.Should().BeOfType<AcceptedAtActionResult>().Subject;
        result.StatusCode.Should().Be((int)HttpStatusCode.Accepted);
        
        var orderInDb = await _dbContext.Orders.FirstOrDefaultAsync(o => o.UserId == userId);
        orderInDb.Should().NotBeNull();
        orderInDb.Amount.Should().Be(amount);
        orderInDb.Status.Should().Be(OrderStatus.Pending);

        var outboxMessageInDb = await _dbContext.OutboxMessages.FirstOrDefaultAsync();
        outboxMessageInDb.Should().NotBeNull();
        outboxMessageInDb.Type.Should().Be("OrderCreated");
        outboxMessageInDb.Payload.Should().Contain(orderInDb.Id.ToString());
    }

    [Fact]
    public async Task GetOrderStatus_WhenOrderExists_ShouldReturnOkWithStatus()
    {
        var orderId = Guid.NewGuid();
        var newOrder = new Order
        {
            Id = orderId,
            UserId = "some-user",
            Amount = 100,
            Status = OrderStatus.Pending
        };
        await _dbContext.Orders.AddAsync(newOrder);
        await _dbContext.SaveChangesAsync();

        var actionResult = await _controller.GetOrderStatus(orderId);

        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value;
        
        value.Should().NotBeNull();
        value.GetType().GetProperty("Id")?.GetValue(value, null).Should().Be(orderId);
        value.GetType().GetProperty("Status")?.GetValue(value, null).Should().Be(OrderStatus.Pending);
    }
    
    [Fact]
    public async Task GetOrderStatus_WhenOrderDoesNotExist_ShouldReturnNotFound()
    {
        var nonExistentOrderId = Guid.NewGuid();

        var actionResult = await _controller.GetOrderStatus(nonExistentOrderId);

        actionResult.Should().BeOfType<NotFoundObjectResult>();
    }
}