using Zoo;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton<IVeterinary, Veterinary>();
services.AddSingleton<IZooReporter, ConsoleReporter>();
services.AddSingleton<ZooGarden>();

var provider = services.BuildServiceProvider();
var zooGarden = provider.GetRequiredService<ZooGarden>();


zooGarden.AddAnimal(new Monkey("Горилыч", 5, 9));
zooGarden.AddAnimal(new Rabbit("Пушистик", 2, 8));  
zooGarden.AddAnimal(new Tiger("Шерхан", 10));     

zooGarden.AddInventory(new Table(101));
zooGarden.AddInventory(new Computer(102));

zooGarden.AddInventory(new Table(2));

zooGarden.ReportInventory();
zooGarden.ReportConsumptions();
zooGarden.ReportAllAnimals();
zooGarden.ReportContactAnimals();
