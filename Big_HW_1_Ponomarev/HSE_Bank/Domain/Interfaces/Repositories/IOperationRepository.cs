using HSE_Bank.Domain.Entities;

namespace HSE_Bank.Domain.Interfaces.Repositories;

public interface IOperationRepository
{
    Operation? GetOperationById(int id);
    IEnumerable<Operation> GetAllOperations();
    IEnumerable<Operation> GetOperationsByAccount(int accountId);
    IEnumerable<Operation> GetOperationsByCategory(int categoryId);
    IEnumerable<Operation> GetOperationsByPeriod(DateTime startDate, DateTime endDate);
    void AddOperation(Operation operation);
    void UpdateOperation(Operation operation);
    void DeleteOperation(int id);
}