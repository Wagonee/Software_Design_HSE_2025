using HSE_Bank.Application.Commands;
using HSE_Bank.Application.Decorators;
using HSE_Bank.Application.Facades;
using HSE_Bank.ConsoleApp;
using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Facades;
using HSE_Bank.Domain.Interfaces.IFactories;
using HSE_Bank.Domain.Interfaces.Repositories;
using HSE_Bank.Infrastructure.Fabrics;
using HSE_Bank.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.AddFileLogging();
});

services.AddSingleton<IDomainObjectFactory, DomainObjectFactory>();
services.AddSingleton<IAccountRepository, InMemoryAccountRepository>(); 
services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
services.AddSingleton<IOperationRepository, InMemoryOperationRepository>();
services.AddSingleton<IAccountFacade, AccountFacade>();
services.AddSingleton<ICategoryFacade, CategoryFacade>();
services.AddSingleton<IOperationFacade, OperationFacade>();
services.AddSingleton<IAnalyticsFacade, AnalyticsFacade>();
services.AddSingleton<CreateAccountCommand>();

var serviceProvider = services.BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

try
{
    var accountFacade = serviceProvider.GetRequiredService<IAccountFacade>();
    var operationFacade = serviceProvider.GetRequiredService<IOperationFacade>();
    var categoryFacade = serviceProvider.GetRequiredService<ICategoryFacade>();
    var factory = serviceProvider.GetRequiredService<IDomainObjectFactory>();
    var accountRepository = serviceProvider.GetRequiredService<IAccountRepository>();
    var operationRepository = serviceProvider.GetRequiredService<IOperationRepository>();
    var categoryRepository = serviceProvider.GetRequiredService<ICategoryRepository>();

    var account1 = accountFacade.CreateAccount("My Account 1", 1000);
    var account2 = accountFacade.CreateAccount("My Account 2", 500);
    var category1 = categoryFacade.CreateCategory("Food", TypeCategory.Expense);
    var category2 = categoryFacade.CreateCategory("Salary", TypeCategory.Income);

    var createAccountCommand = new CreateAccountCommand(accountFacade, 200,
        "Test Account");
    var decoratedCommand = new ExecutionTimeDecorator(createAccountCommand,
        serviceProvider.GetRequiredService<ILogger<ExecutionTimeDecorator>>());

    decoratedCommand.Execute(); 

    logger.LogInformation($"Created account: {account1}");
    logger.LogInformation($"Created category: {category1}");

    Console.WriteLine();
    var operation1 = operationFacade.CreateOperation(TypeCategory.Expense, 50,
        DateTime.Now, account1.Id, category1.Id, "Lunch");
    logger.LogInformation($"Created operation: {operation1}");

    var analyticsFacade = serviceProvider.GetRequiredService<IAnalyticsFacade>();
    var difference = analyticsFacade.GetDifferenceBetweenIncomeAndExpenses(
        DateTime.Now.AddDays(-30), DateTime.Now);
    logger.LogInformation($"Difference between income and expenses: {difference}");

    var allAccounts = accountFacade.GetAllAccounts();
    foreach (var account in allAccounts)
    {
        logger.LogInformation($"Account: {account}");
    }
}
catch (Exception ex)
{
    logger.LogError(ex.Message, "An error occurred");
}

logger.LogInformation("Application finished");