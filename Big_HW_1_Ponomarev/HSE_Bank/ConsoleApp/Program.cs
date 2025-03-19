using Microsoft.Extensions.DependencyInjection;
using HSE_Bank.Application.Facades;
using HSE_Bank.ConsoleApp;
using HSE_Bank.Infrastructure.Repositories;
using HSE_Bank.ConsoleApp.Menus;
using HSE_Bank.Domain.Interfaces.IFactories;
using HSE_Bank.Domain.Interfaces.Repositories;
using HSE_Bank.Infrastructure.Fabrics;

var serviceProvider = new ServiceCollection()
    .AddSingleton<IDomainObjectFactory, DomainObjectFactory>()
    .AddSingleton<IAccountRepository, InMemoryAccountRepository>()
    .AddSingleton<ICategoryRepository, InMemoryCategoryRepository>()
    .AddSingleton<IOperationRepository, InMemoryOperationRepository>()
    .AddSingleton<AccountFacade>()
    .AddSingleton<CategoryFacade>()
    .AddSingleton<OperationFacade>()
    .BuildServiceProvider();

AccountMenu.Init(serviceProvider.GetRequiredService<AccountFacade>());
CategoryMenu.Init(serviceProvider.GetRequiredService<CategoryFacade>());
OperationMenu.Init(serviceProvider.GetRequiredService<OperationFacade>(), serviceProvider.GetRequiredService<AccountFacade>(), serviceProvider.GetRequiredService<CategoryFacade>());

MainMenu.Show();