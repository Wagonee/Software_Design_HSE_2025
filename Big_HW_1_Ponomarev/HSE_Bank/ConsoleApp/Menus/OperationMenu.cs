using HSE_Bank.Application.Facades;
using HSE_Bank.Application.Commands;
using HSE_Bank.ConsoleApp.Utils;

namespace HSE_Bank.ConsoleApp.Menus
{
    public static class OperationMenu
    {
        private static OperationFacade? _operationFacade;
        private static AccountFacade? _accountFacade;
        private static CategoryFacade? _categoryFacade;

        public static void Init(OperationFacade operationFacade, AccountFacade accountFacade, CategoryFacade categoryFacade)
        {
            _operationFacade = operationFacade;
            _accountFacade = accountFacade;
            _categoryFacade = categoryFacade;
        }

        public static void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("====== Управление операциями ======");
                Console.WriteLine("1. Создать операцию");
                Console.WriteLine("2. Удалить операцию");
                Console.WriteLine("3. Показать все операции");
                Console.WriteLine("4. Вернуться в главное меню");

                int choice = ConsoleHelper.GetIntInput("Выберите действие (1-4): ", 1, 4);

                switch (choice)
                {
                    case 1:
                        CreateOperation();
                        break;
                    case 2:
                        DeleteOperation();
                        break;
                    case 3:
                        ShowAllOperations();
                        break;
                    case 4:
                        return;
                }
            }
        }

        private static void CreateOperation()
        {
            Console.Clear();
            Console.WriteLine("====== Создание новой операции ======");

            ShowAllAccounts();
            int accountId = ConsoleHelper.GetIntInput("Введите ID счета: ", 1, int.MaxValue);

            ShowAllCategories();
            int categoryId = ConsoleHelper.GetIntInput("Введите ID категории: ", 1, int.MaxValue);
            decimal amount = ConsoleHelper.GetDecimalInput("Введите сумму операции: ");
            Console.Write("Введите описание операции (необязательно): ");
            string description = Console.ReadLine();

            var account = _accountFacade.GetAccountById(accountId);
            var category = _categoryFacade.GetCategoryById(categoryId);

            if (account == null || category == null)
            {
                Console.WriteLine("Ошибка: Некорректные данные. Проверьте ID счета и категории.");
            }
            else
            {
                if (account.Balance < amount)
                {
                    Console.WriteLine("Баланс не может быть отрицательным!");
                }
                else
                {
                    var operationCreateCmd = new CreateOperationCommand(_operationFacade, category.Type, amount,
                        DateTime.Now, account.Id, category.Id, description);
                    operationCreateCmd.Execute();
                    Console.WriteLine(
                        $"Операция добавлена: {amount} {category.Type} на {account.Name} в категории {category.Name}");
                }
            }

            ConsoleHelper.WaitForKey();
        }

        private static void DeleteOperation()
        {
            Console.Clear();
            Console.WriteLine("====== Удаление операции ======");
            ShowAllOperations();

            int operationId = ConsoleHelper.GetIntInput("Введите ID операции для удаления: ", 1, int.MaxValue);
            
            var operation = _operationFacade.GetOperationById(operationId);
            if (operation == null)
            {
                Console.WriteLine("Ошибка: Операция не найдена.");
            }
            else
            {
                var deleteOperationCmd = new DeleteOperationCommand(_operationFacade, operationId);
                deleteOperationCmd.Execute();
                Console.WriteLine($"Операция с ID {operationId} удалена.");
            }

            ConsoleHelper.WaitForKey();
        }

        private static void ShowAllOperations()
        {
            Console.Clear();
            Console.WriteLine("====== Список операций ======");
            var operations = _operationFacade.GetAllOperations();

            if (operations == null || operations.Count() == 0)
            {
                Console.WriteLine("Нет доступных операций.");
            }
            else
            {
                foreach (var operation in operations)
                {
                    var account = _accountFacade.GetAccountById(operation.BankAccountId);
                    var category = _categoryFacade.GetCategoryById(operation.CategoryId);
                    Console.WriteLine($"ID: {operation.Id} | {operation.Type} | {operation.Amount} | Счет: {account?.Name} | Категория: {category?.Name}");
                }
            }

            ConsoleHelper.WaitForKey();
        }

        private static void ShowAllAccounts()
        {
            var accounts = _accountFacade.GetAllAccounts();
            Console.WriteLine("Доступные счета:");
            foreach (var account in accounts)
            {
                Console.WriteLine($"ID: {account.Id} | {account.Name} | Баланс: {account.Balance}");
            }
        }

        private static void ShowAllCategories()
        {
            var categories = _categoryFacade.GetAllCategories();
            Console.WriteLine("Доступные категории:");
            foreach (var category in categories)
            {
                Console.WriteLine($"ID: {category.Id} | {category.Name} | Тип: {category.Type}");
            }
        }
    }
}
