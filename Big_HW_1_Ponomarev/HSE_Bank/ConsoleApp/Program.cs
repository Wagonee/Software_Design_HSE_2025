using HSE_Bank.Application.Commands;
using HSE_Bank.Application.Facades;
using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Facades;
using HSE_Bank.Domain.Interfaces.IFactories;
using HSE_Bank.Domain.Interfaces.Repositories;
using HSE_Bank.Infrastructure.Fabrics;
using HSE_Bank.Infrastructure.Repositories;
using HSE_Bank.Infrastructure.Exporters;
using HSE_Bank.Infrastructure.Importers;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddSingleton<IDomainObjectFactory, DomainObjectFactory>();
services.AddSingleton<IAccountRepository, InMemoryAccountRepository>();
services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
services.AddSingleton<IOperationRepository, InMemoryOperationRepository>();
services.AddSingleton<IAccountFacade, AccountFacade>();
services.AddSingleton<ICategoryFacade, CategoryFacade>();
services.AddSingleton<IOperationFacade, OperationFacade>();
services.AddSingleton<IAnalyticsFacade, AnalyticsFacade>();

var serviceProvider = services.BuildServiceProvider();

try
{
    var accountFacade = serviceProvider.GetRequiredService<IAccountFacade>();
    var categoryFacade = serviceProvider.GetRequiredService<ICategoryFacade>();
    var operationFacade = serviceProvider.GetRequiredService<IOperationFacade>();

    Console.WriteLine("=== Тест команд ===");
    var createAccountCmd = new CreateAccountCommand(accountFacade, 1000, "Основной счет");
    createAccountCmd.Execute();
    var account = accountFacade.GetAllAccounts().FirstOrDefault();
    Console.WriteLine($"Создан счет: {account}");

    var createCategoryCmd = new CreateCategoryCommand(categoryFacade, "Продукты", TypeCategory.Expense);
    createCategoryCmd.Execute();
    var category = categoryFacade.GetAllCategories().FirstOrDefault();
    Console.WriteLine($"Создана категория: {category}");

    var createOperationCmd = new CreateOperationCommand(operationFacade, TypeCategory.Expense, 200, DateTime.Now, account.Id, category.Id, "Покупка еды");
    createOperationCmd.Execute();
    var operation = operationFacade.GetAllOperations().FirstOrDefault();
    Console.WriteLine($"Создана операция: {operation}");

    var jsonExporter = new JsonDataExporter();
    var csvExporter = new CsvDataExporter();

    Console.WriteLine("\n=== Экспорт данных в файлы (JSON, CSV) ===");
    
    var allAccounts = accountFacade.GetAllAccounts();
    var allCategories = categoryFacade.GetAllCategories();
    var allOperations = operationFacade.GetAllOperations();

    foreach (var acc in allAccounts)
    {
        acc.Accept(jsonExporter);
        acc.Accept(csvExporter);
    }

    foreach (var cat in allCategories)
    {
        cat.Accept(jsonExporter);
        cat.Accept(csvExporter);
    }

    foreach (var op in allOperations)
    {
        op.Accept(jsonExporter);
        op.Accept(csvExporter);
    }

    var jsonAccountImporter = new JsonDataImporter<BankAccount>();
    var csvCategoryImporter = new CsvDataImporter<Category>();
    var jsonOperationImporter = new JsonDataImporter<Operation>();

    Console.WriteLine("\n=== Импорт данных из файлов (JSON, CSV) ===");

    var importedAccounts = jsonAccountImporter.Import("accounts.json");
    var importedCategories = csvCategoryImporter.Import("categories.csv");
    var importedOperations = jsonOperationImporter.Import("operations.json");

    foreach (var acc in importedAccounts)
    {
        Console.WriteLine($"Импортирован счет: {acc}");
    }

    foreach (var cat in importedCategories)
    {
        Console.WriteLine($"Импортирована категория: {cat}");
    }

    foreach (var op in importedOperations)
    {
        Console.WriteLine($"Импортирована операция: {op}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка во время выполнения программы: {ex.Message}");
}

Console.WriteLine("=== Завершение работы ===");
