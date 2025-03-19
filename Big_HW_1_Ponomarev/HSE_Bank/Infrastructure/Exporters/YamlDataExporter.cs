using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Exporters;
using YamlDotNet.Serialization;

namespace HSE_Bank.Infrastructure.Exporters;

public class YamlDataExporter : IDataExporter
{
    private readonly ISerializer _serializer = new SerializerBuilder().Build();
    public string ExportBankAccount(BankAccount account) => _serializer.Serialize(account);
    public string ExportCategory(Category category) => _serializer.Serialize(category);
    public string ExportOperation(Operation operation) => _serializer.Serialize(operation);
}