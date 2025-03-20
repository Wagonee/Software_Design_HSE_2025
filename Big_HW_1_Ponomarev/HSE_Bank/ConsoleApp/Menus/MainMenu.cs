using HSE_Bank.ConsoleApp.Utils;

namespace HSE_Bank.ConsoleApp.Menus
{
    public static class MainMenu
    {
        public static void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("====== ВШЭ-БАНК: Главное меню ======");
                Console.WriteLine("1. Управление счетами");
                Console.WriteLine("2. Управление категориями");
                Console.WriteLine("3. Управление операциями");
                Console.WriteLine("4. Аналитика");
                Console.WriteLine("5. Импорт и экспорт данных");
                Console.WriteLine("6. Выход");
                var choice = ConsoleHelper.GetIntInput("Выберите действие (1-5): ", 1, 6);

                switch (choice)
                {
                    case 1:
                        AccountMenu.Show();
                        break;
                    case 2:
                        CategoryMenu.Show();
                        break;
                    case 3:
                        OperationMenu.Show();
                        break;
                    case 4:
                        AnalyticsMenu.Show();
                        break;
                    case 5:
                        ImportExportMenu.Show();
                        break;
                    case 6:
                        Console.WriteLine("Выход из программы...");
                        return;
                }
            }
        }
    }
}