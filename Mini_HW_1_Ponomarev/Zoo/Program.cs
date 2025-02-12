using Zoo;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton<IVeterinary, Veterinary>();
services.AddSingleton<IZooReporter, ConsoleReporter>();
services.AddSingleton<ZooGarden>();

var provider = services.BuildServiceProvider();
var zooGarden = provider.GetRequiredService<ZooGarden>();

zooGarden.AddAnimal(new Monkey("Горилыч", 5, 90, 6));
zooGarden.AddAnimal(new Rabbit("Заяц", 2, 80, 4));  
zooGarden.AddAnimal(new Tiger("Тигр Тигрович", 10, 100));     
zooGarden.AddAnimal(new Wolf("Зуб", 9, 20));

zooGarden.AddInventory(new Table(101));
zooGarden.AddInventory(new Computer(102));
zooGarden.AddInventory(new Table(2));

zooGarden.ReportInventory();
Console.WriteLine();
zooGarden.ReportConsumptions();
Console.WriteLine();
zooGarden.ReportAllAnimals();
Console.WriteLine();
zooGarden.ReportContactAnimals();



