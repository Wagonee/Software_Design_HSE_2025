using HSE_Bank.Application.Facades;
using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Repositories;
using HSE_Bank.Infrastructure.Fabrics;
using Moq;

namespace HSE_Bank_Tests;

public class AnalyticsFacadeTests
{
    private readonly Mock<IOperationRepository> _operationRepositoryMock;
    private readonly AnalyticsFacade _analyticsFacade;
    private readonly DomainObjectFactory _factory;

    public AnalyticsFacadeTests()
    {
        _operationRepositoryMock = new Mock<IOperationRepository>();
        _analyticsFacade = new AnalyticsFacade(_operationRepositoryMock.Object);
        _factory = new DomainObjectFactory();
    }

    [Fact]
    public void GetDifferenceBetweenIncomeAndExpenses_ShouldReturnZero_WhenNoOperations()
    {
        _operationRepositoryMock.Setup(repo => repo.GetAllOperations()).Returns(new List<Operation>());
        Assert.Equal(0, _analyticsFacade.GetDifferenceBetweenIncomeAndExpenses());
    }

    [Fact]
    public void GetDifferenceBetweenIncomeAndExpenses_ShouldReturnIncomeSum_WhenOnlyIncomeExists()
    {
        var operations = new List<Operation>
        {
            _factory.CreateOperation(1, TypeCategory.Income, 500, DateTime.Now, 1, 1)
        };
        _operationRepositoryMock.Setup(repo => repo.GetAllOperations()).Returns(operations);
        Assert.Equal(500, _analyticsFacade.GetDifferenceBetweenIncomeAndExpenses());
    }

    [Fact]
    public void GetDifferenceBetweenIncomeAndExpenses_ShouldReturnNegativeExpenseSum_WhenOnlyExpensesExist()
    {
        var operations = new List<Operation>
        {
            _factory.CreateOperation(1, TypeCategory.Expense, 300, DateTime.Now, 1, 1)
        };
        _operationRepositoryMock.Setup(repo => repo.GetAllOperations()).Returns(operations);
        Assert.Equal(-300, _analyticsFacade.GetDifferenceBetweenIncomeAndExpenses());
    }

    [Fact]
    public void GetDifferenceBetweenIncomeAndExpenses_ShouldReturnCorrectDifference_WhenIncomeAndExpensesExist()
    {
        var operations = new List<Operation>
        {
            _factory.CreateOperation(1, TypeCategory.Income, 500, DateTime.Now, 1, 1),
            _factory.CreateOperation(2, TypeCategory.Expense, 200, DateTime.Now, 1, 1)
        };
        _operationRepositoryMock.Setup(repo => repo.GetAllOperations()).Returns(operations);
        Assert.Equal(300, _analyticsFacade.GetDifferenceBetweenIncomeAndExpenses());
    }

    [Fact]
    public void GroupOperationsByCategory_ShouldReturnEmptyCollection_WhenNoOperationsExist()
    {
        _operationRepositoryMock.Setup(repo => repo.GetAllOperations()).Returns(new List<Operation>());
        var result = _analyticsFacade.GroupOperationsByCategory();
        Assert.Empty(result);
    }

    [Fact]
    public void GroupOperationsByCategory_ShouldGroupCorrectly_WhenOperationsExist()
    {
        var operations = new List<Operation>
        {
            _factory.CreateOperation(1, TypeCategory.Income, 500, DateTime.Now, 1, 1),
            _factory.CreateOperation(2, TypeCategory.Income, 300, DateTime.Now, 1, 1),
            _factory.CreateOperation(3, TypeCategory.Expense, 200, DateTime.Now, 1, 1)
        };
        _operationRepositoryMock.Setup(repo => repo.GetAllOperations()).Returns(operations);
        
        var result = _analyticsFacade.GroupOperationsByCategory().ToList();
        
        Assert.Equal(2, result.Count);
        Assert.Single(result.First(g => g.Key == TypeCategory.Expense));
        Assert.Equal(2, result.First(g => g.Key == TypeCategory.Income).Count());
    }
}
