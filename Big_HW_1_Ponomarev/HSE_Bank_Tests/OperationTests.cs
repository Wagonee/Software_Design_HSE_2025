using HSE_Bank.Domain.Entities;
using HSE_Bank.Infrastructure.Fabrics;

namespace HSE_Bank_Tests;

public class OperationTests
{
    private readonly DomainObjectFactory _factory = new();

    [Fact]
    public void CreateOperation_ShouldCreateValidOperation()
    {
        var operation = _factory.CreateOperation(1, TypeCategory.Expense, 200, DateTime.Now, 1, 2, "Dinner");
        Assert.Equal(1, operation.Id);
        Assert.Equal(TypeCategory.Expense, operation.Type);
        Assert.Equal(200, operation.Amount);
        Assert.Equal(1, operation.BankAccountId);
        Assert.Equal(2, operation.CategoryId);
        Assert.Equal("Dinner", operation.Description);
    }

    [Fact]
    public void CreateOperation_ShouldThrowException_WhenAmountIsNegative()
    {
        Assert.Throws<ArgumentException>(() => _factory.CreateOperation(1, TypeCategory.Expense, -200, DateTime.Now, 1, 2, "Dinner"));
    }
}