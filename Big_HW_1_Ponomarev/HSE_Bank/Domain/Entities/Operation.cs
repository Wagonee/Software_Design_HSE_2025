namespace HSE_Bank.Domain.Entities;

public class Operation
{
    public int Id { get; private set; }
    public TypeCategory Type { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string? Description { get; private set; }
    
    public int CategoryId { get; private set; }
    public int BankAccountId { get; private set; }
    
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

    public override string ToString()
    {
        return $"Id: {Id}, Type: {Type}, Amount: {Amount}, Date: {Date}";
    }
}