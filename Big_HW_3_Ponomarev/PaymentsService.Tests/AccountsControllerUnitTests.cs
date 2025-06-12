using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentsService.Controllers;
using PaymentsService.Data;
using PaymentsService.Data.Entities;
using PaymentsService.Dtos;

namespace PaymentsService.Tests;

public class AccountsControllerUnitTests
{
    private readonly PaymentsDbContext _dbContext;
    private readonly AccountsController _controller;

    public AccountsControllerUnitTests()
    {
        var options = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseInMemoryDatabase($"TestDb-{Guid.NewGuid()}") 
            .Options;
            
        _dbContext = new PaymentsDbContext(options);

        var loggerMock = new Mock<ILogger<AccountsController>>();

        _controller = new AccountsController(_dbContext, loggerMock.Object);
    }

    [Fact]
    public async Task CreateAccount_WhenUserIdIsValid_ShouldReturnCreatedResult()
    {
        var userId = "new-test-user";

        var actionResult = await _controller.CreateAccount(userId);

        actionResult.Should().BeOfType<ActionResult<AccountDto>>();
        
        var result = actionResult.Result as CreatedAtActionResult;
        result.Should().NotBeNull();
        result.StatusCode.Should().Be((int)HttpStatusCode.Created);

        var createdDto = result.Value as AccountDto;
        createdDto.Should().NotBeNull();
        createdDto.UserId.Should().Be(userId);
        createdDto.Balance.Should().Be(0);

        var accountInDb = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
        accountInDb.Should().NotBeNull();
    }
    
    [Fact]
    public async Task CreateAccount_WhenUserAlreadyExists_ShouldReturnConflict()
    {
        var userId = "existing-user-for-conflict";
        
        _dbContext.Accounts.Add(new Account { UserId = userId, Balance = 100 });
        await _dbContext.SaveChangesAsync();

        var actionResult = await _controller.CreateAccount(userId);

        actionResult.Result.Should().BeOfType<ConflictObjectResult>();
        var result = actionResult.Result as ConflictObjectResult;
        result?.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Deposit_WhenAccountExists_ShouldIncreaseBalance()
    {
        var userId = "user-for-deposit";
        var initialBalance = 100m;
        var depositAmount = 50m;

        _dbContext.Accounts.Add(new Account { UserId = userId, Balance = initialBalance });
        await _dbContext.SaveChangesAsync();
        
        var actionResult = await _controller.Deposit(userId, depositAmount);
        
        actionResult.Result.Should().BeOfType<OkObjectResult>();
        var okResult = actionResult.Result as OkObjectResult;
        var accountDto = okResult?.Value as AccountDto;
        
        accountDto?.Balance.Should().Be(initialBalance + depositAmount);
    }
}