using HSE_Bank.Application.Commands;
using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Facades;
using HSE_Bank.Infrastructure.Fabrics;
using Moq;

namespace HSE_Bank_Tests;

public class CommandTests
{
    private readonly Mock<IAccountFacade> _accountFacadeMock;
    private readonly Mock<ICategoryFacade> _categoryFacadeMock;
    private readonly Mock<IOperationFacade> _operationFacadeMock;
    private readonly DomainObjectFactory _factory;

    public CommandTests()
    {
        _accountFacadeMock = new Mock<IAccountFacade>();
        _categoryFacadeMock = new Mock<ICategoryFacade>();
        _operationFacadeMock = new Mock<IOperationFacade>();
        _factory = new DomainObjectFactory();
    }

    [Fact]
    public void CreateAccountCommand_ShouldCallFacadeWithCorrectData()
    {
        var command = new CreateAccountCommand(_accountFacadeMock.Object, 100, "Test Account");
        command.Execute();
        
        _accountFacadeMock.Verify(f => f.CreateAccount(It.Is<string>(s => s == "Test Account"), 
                                                        It.Is<decimal>(b => b == 100)), 
                                                        Times.Once);
    }

    [Fact]
    public void CreateCategoryCommand_ShouldCallFacadeWithCorrectData()
    {
        var command = new CreateCategoryCommand(_categoryFacadeMock.Object, "Salary", TypeCategory.Income);
        command.Execute();
        
        _categoryFacadeMock.Verify(f => f.CreateCategory(It.Is<string>(s => s == "Salary"), 
                                                          It.Is<TypeCategory>(t => t == TypeCategory.Income)), 
                                                          Times.Once);
    }

    [Fact]
    public void CreateOperationCommand_ShouldCallFacadeWithCorrectData()
    {
        var command = new CreateOperationCommand(_operationFacadeMock.Object, TypeCategory.Expense, 50, DateTime.Now, 1, 2, "Lunch");
        command.Execute();
        
        _operationFacadeMock.Verify(f => f.CreateOperation(It.Is<TypeCategory>(t => t == TypeCategory.Expense),
                                                           It.Is<decimal>(a => a == 50),
                                                           It.IsAny<DateTime>(),
                                                           It.Is<int>(b => b == 1),
                                                           It.Is<int>(c => c == 2),
                                                           It.Is<string>(d => d == "Lunch")), 
                                                           Times.Once);
    }

    [Fact]
    public void DeleteAccountCommand_ShouldCallFacadeWithCorrectId()
    {
        var command = new DeleteAccountCommand(_accountFacadeMock.Object, 1);
        command.Execute();
        
        _accountFacadeMock.Verify(f => f.DeleteAccount(It.Is<int>(id => id == 1)), Times.Once);
    }

    [Fact]
    public void UpdateCategoryCommand_ShouldCallFacadeWithCorrectData()
    {
        var category = _factory.CreateCategory(1, "Bonus", TypeCategory.Income);
        var command = new UpdateCategoryCommand(_categoryFacadeMock.Object, category);
        command.Execute();
        
        _categoryFacadeMock.Verify(f => f.UpdateCategory(It.Is<Category>(c => c.Id == 1 && c.Name == "Bonus" && c.Type == TypeCategory.Income)), 
                                                          Times.Once);
    }

    [Fact]
    public void UpdateOperationCommand_ShouldCallFacadeWithCorrectData()
    {
        var operation = _factory.CreateOperation(1, TypeCategory.Expense, 200, DateTime.Now, 1, 2, "Dinner");
        var command = new UpdateOperationCommand(_operationFacadeMock.Object, operation);
        command.Execute();
        
        _operationFacadeMock.Verify(f => f.UpdateOperation(It.Is<Operation>(o => o.Id == 1 && o.Amount == 200 && o.Description == "Dinner")), 
                                                          Times.Once);
    }

    [Fact]
    public void DifferenceAnalyticsCommand_ShouldCallAnalyticsFacade()
    {
        var analyticsFacadeMock = new Mock<IAnalyticsFacade>();
        var command = new DifferenceAnalyticsCommand(analyticsFacadeMock.Object);
        command.Execute();
        
        analyticsFacadeMock.Verify(f => f.GetDifferenceBetweenIncomeAndExpenses(), Times.Once);
    }
}
