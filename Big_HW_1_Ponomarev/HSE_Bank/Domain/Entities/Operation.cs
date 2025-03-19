using HSE_Bank.Domain.Interfaces.Exporters;

namespace HSE_Bank.Domain.Entities;

public class Operation
{
    public int Id { get; set; }
    public TypeCategory Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    
    public int CategoryId { get; set; }
    public int BankAccountId { get; set; }
    
    internal Operation(int id, TypeCategory type, decimal amount, DateTime date,
        int bankAccountId, int categoryId, string? description = null)
    {
        Id = id;
        Type = type;
        Amount = amount;
        Date = date;
        BankAccountId = bankAccountId;
        CategoryId = categoryId;
        Description = description;
    }
    
    public string Accept(IDataExporter visitor) => visitor.ExportOperation(this);
    
    public override string ToString()
    {
        return $"Id: {Id}, Type: {Type}, Amount: {Amount}, Date: {Date}";
    }
}