using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Repositories;

namespace HSE_Bank.Infrastructure.Repositories;

public class InMemoryOperationRepository : IOperationRepository
{
    private readonly List<Operation> _operations = new List<Operation>();

    public Operation? GetOperationById(int id)
    {
        return _operations.FirstOrDefault(x => x.Id == id);
    }

    public IEnumerable<Operation> GetAllOperations()
    {
        return _operations;
    }

    public IEnumerable<Operation> GetOperationsByAccount(int userId)
    {
        return _operations.Where(o => o.BankAccountId == userId);
    }
    
    public IEnumerable<Operation> GetOperationsByCategory(int categoryId)
    {
        return _operations.Where(o => o.CategoryId == categoryId);
    }

    public IEnumerable<Operation> GetOperationsByPeriod(DateTime startDate, DateTime endDate)
    {
        return _operations.Where(o => o.Date >= startDate && o.Date <= endDate);
    }
    
    public void AddOperation(Operation operation)
    {
        _operations.Add(operation);
    }
    public void UpdateOperation(Operation operation)
    {
        int index = _operations.FindIndex(x => x.Id == operation.Id);
        if (index != -1)
        {
            _operations[index] = operation;
        }
    }
    
    public void DeleteOperation(int id)
    {
        var operationToRemove = _operations.FirstOrDefault(o => o.Id == id);
        if (operationToRemove != null)
        {
            _operations.Remove(operationToRemove);
        }
    }
}