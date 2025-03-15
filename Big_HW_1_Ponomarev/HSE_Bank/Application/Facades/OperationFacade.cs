using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Facades;
using HSE_Bank.Domain.Interfaces.Repositories;
using HSE_Bank.Infrastructure.Fabrics;

namespace HSE_Bank.Application.Facades;

public class OperationFacade : IOperationFacade
{
    private readonly DomainObjectFactory _factory;
    private readonly IOperationRepository _operationRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICategoryRepository _categoryRepository;

    public OperationFacade(DomainObjectFactory factory, IOperationRepository operationRepository, IAccountRepository accountRepository, ICategoryRepository categoryRepository)
    {
        _factory = factory;
        _operationRepository = operationRepository;
        _accountRepository = accountRepository;
        _categoryRepository = categoryRepository;
    }

    public Operation? GetOperationById(int id)
    {
        return _operationRepository.GetOperationById(id);
    }

    public IEnumerable<Operation> GetAllOperations()
    {
        return _operationRepository.GetAllOperations();
    }

    public IEnumerable<Operation> GetOperationsByAccount(int accountId)
    {
        return _operationRepository.GetOperationsByAccount(accountId);
    }

    public IEnumerable<Operation> GetOperationsByCategory(int categoryId)
    {
        return _operationRepository.GetOperationsByCategory(categoryId);
    }

    public IEnumerable<Operation> GetOperationsByPeriod(DateTime startDate, DateTime endDate)
    {
        return _operationRepository.GetOperationsByPeriod(startDate, endDate);
    }

    public Operation CreateOperation(TypeCategory type, decimal amount, DateTime date, int bankAccountId, int categoryId,
        string? description = null)
    {
        if (_accountRepository.GetAccount(bankAccountId) == null)
        {
            throw new ArgumentException("Account not found");
        }

        if (_categoryRepository.GetCategoryById(categoryId) == null)
        {
            throw new ArgumentException("Category not found");
        }
        int id = new Random().Next();
        var operation = _factory.CreateOperation(id, type, amount, date, bankAccountId, categoryId, description);
        _operationRepository.AddOperation(operation);
        return operation;
    }

    public void UpdateOperation(Operation operation)
    {
        _operationRepository.UpdateOperation(operation);
    }

    public void DeleteOperation(int id)
    {
        _operationRepository.DeleteOperation(id);
    }
}