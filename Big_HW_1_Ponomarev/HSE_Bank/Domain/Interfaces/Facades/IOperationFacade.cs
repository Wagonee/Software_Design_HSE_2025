using HSE_Bank.Domain.Entities;

namespace HSE_Bank.Domain.Interfaces.Facades;

public interface IOperationFacade
{
    Operation? GetOperationById(int id);
    IEnumerable<Operation> GetAllOperations();
    IEnumerable<Operation> GetOperationsByAccount(int accountId);
    IEnumerable<Operation> GetOperationsByCategory(int categoryId);
    IEnumerable<Operation> GetOperationsByPeriod(DateTime startDate, DateTime endDate);
    Operation CreateOperation(TypeCategory type, decimal amount, DateTime date,
        int bankAccountId, int categoryId, string? description = null);
    void UpdateOperation(Operation operation);
    void DeleteOperation(int id);
}