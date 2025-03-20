using HSE_Bank.Domain.Entities;
using HSE_Bank.Infrastructure.Repositories;
using HSE_Bank.Infrastructure.Fabrics;

namespace HSE_Bank_Tests;

public class RepositoryTests
{
    private readonly DomainObjectFactory _factory = new();

    [Fact]
    public void InMemoryAccountRepository_ShouldAddAndRetrieveAccount()
    {
        var repository = new InMemoryAccountRepository();
        var account = _factory.CreateBankAccount("Test Account", 100, 1);
        repository.AddAccount(account);

        var retrieved = repository.GetAccount(1);
        Assert.NotNull(retrieved);
        Assert.Equal("Test Account", retrieved.Name);
    }

    [Fact]
    public void InMemoryCategoryRepository_ShouldAddAndRetrieveCategory()
    {
        var repository = new InMemoryCategoryRepository();
        var category = _factory.CreateCategory(1, "Food", TypeCategory.Expense);
        repository.AddCategory(category);

        var retrieved = repository.GetCategoryById(1);
        Assert.NotNull(retrieved);
        Assert.Equal("Food", retrieved.Name);
    }

    [Fact]
    public void InMemoryOperationRepository_ShouldAddAndRetrieveOperation()
    {
        var repository = new InMemoryOperationRepository();
        var operation = _factory.CreateOperation(1, TypeCategory.Income, 200, DateTime.Now, 1, 1, "Salary");
        repository.AddOperation(operation);

        var retrieved = repository.GetOperationById(1);
        Assert.NotNull(retrieved);
        Assert.Equal(200, retrieved.Amount);
    }

    [Fact]
    public void InMemoryAccountRepository_ShouldUpdateAccount()
    {
        var repository = new InMemoryAccountRepository();
        var account = _factory.CreateBankAccount("Old Name", 100, 1);
        repository.AddAccount(account);

        repository.UpdateAccount(1, "New Name");
        var updated = repository.GetAccount(1);
        Assert.Equal("New Name", updated.Name);
    }

    [Fact]
    public void InMemoryCategoryRepository_ShouldDeleteCategory()
    {
        var repository = new InMemoryCategoryRepository();
        var category = _factory.CreateCategory(1, "Entertainment", TypeCategory.Expense);
        repository.AddCategory(category);

        repository.DeleteCategory(1);
        Assert.Null(repository.GetCategoryById(1));
    }

    [Fact]
    public void InMemoryOperationRepository_ShouldRetrieveOperationsByAccount()
    {
        var repository = new InMemoryOperationRepository();
        var operation1 = _factory.CreateOperation(1, TypeCategory.Expense, 50, DateTime.Now, 1, 2, "Groceries");
        var operation2 = _factory.CreateOperation(2, TypeCategory.Income, 500, DateTime.Now, 1, 3, "Salary");
        repository.AddOperation(operation1);
        repository.AddOperation(operation2);

        var operations = repository.GetOperationsByAccount(1).ToList();
        Assert.Equal(2, operations.Count);
    }
}
