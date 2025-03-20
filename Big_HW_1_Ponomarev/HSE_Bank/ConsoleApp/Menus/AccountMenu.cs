using HSE_Bank.Application.Facades;
using HSE_Bank.Application.Commands;
using HSE_Bank.ConsoleApp.Utils;

namespace HSE_Bank.ConsoleApp.Menus
{
    public static class AccountMenu
    {
        private static AccountFacade _accountFacade;

        public static void Init(AccountFacade accountFacade)
        {
            _accountFacade = accountFacade;
        }

        public static void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("====== Управление счетами ======");
                Console.WriteLine("1. Создать счет");
                Console.WriteLine("2. Удалить счет");
                Console.WriteLine("3. Показать все счета");
                Console.WriteLine("4. Редактировать счёт");
                Console.WriteLine("5. Вернуться в главное меню");

                int choice = ConsoleHelper.GetIntInput("Выберите действие (1-5): ", 1, 5);

                switch (choice)
                {
                    case 1:
                        CreateAccount();
                        break;
                    case 2:
                        DeleteAccount();
                        break;
                    case 3:
                        ShowAllAccounts();
                        break;
                    case 4:
                        UpdateAccount();
                        break;
                    case 5:
                        return;
                }
            }
        }

        private static void UpdateAccount()
        {
            Console.Clear();
            Console.WriteLine("====== Редактирование существующего счёта ======");
            ShowAllAccounts();

            int accountId = ConsoleHelper.GetIntInput("Введите ID счета для редактирования: ", 1, int.MaxValue);

            var account = _accountFacade.GetAccountById(accountId);
            if (account == null)
            {
                Console.WriteLine("Ошибка: Счет не найден.");
            }
            else
            {
                string? newName = ConsoleHelper.GetStringInput("Введите новое имя для счёта: ");
                var deleteAccCmd = new UpdateAccountCommand(_accountFacade, accountId, new string(newName ?? string.Empty));
                deleteAccCmd.Execute();
                Console.WriteLine($"Счет '{account.Name}' изменён.");
            }

            ConsoleHelper.WaitForKey();
            
        }
        private static void CreateAccount()
        {
            Console.Clear();
            Console.WriteLine("====== Создание нового счета ======");
            string name = ConsoleHelper.GetStringInput("Введите название счета: ");
            decimal balance = ConsoleHelper.GetDecimalInput("Введите начальный баланс: ");
            
            var createAccCmd = new CreateAccountCommand(_accountFacade, balance, name);
            createAccCmd.Execute();
            Console.WriteLine($"Счет '{name}' создан с балансом {balance}.");
            ConsoleHelper.WaitForKey();
        }

        private static void DeleteAccount()
        {
            Console.Clear();
            Console.WriteLine("====== Удаление счета ======");
            ShowAllAccounts();

            int accountId = ConsoleHelper.GetIntInput("Введите ID счета для удаления: ", 1, int.MaxValue);

            var account = _accountFacade.GetAccountById(accountId);
            if (account == null)
            {
                Console.WriteLine("Ошибка: Счет не найден.");
            }
            else
            {
                var deleteAccCmd = new DeleteAccountCommand(_accountFacade, accountId);
                deleteAccCmd.Execute();
                Console.WriteLine($"Счет '{account.Name}' удален.");
            }

            ConsoleHelper.WaitForKey();
        }

        private static void ShowAllAccounts()
        {
            Console.Clear();
            Console.WriteLine("====== Список счетов ======");
            var accounts = _accountFacade.GetAllAccounts();

            if (accounts == null || accounts.Count() == 0)
            {
                Console.WriteLine("Нет доступных счетов.");
            }
            else
            {
                foreach (var account in accounts)
                {
                    Console.WriteLine($"ID: {account.Id} | Название: {account.Name} | Баланс: {account.Balance}");
                }
            }

            ConsoleHelper.WaitForKey();
        }
    }
}
