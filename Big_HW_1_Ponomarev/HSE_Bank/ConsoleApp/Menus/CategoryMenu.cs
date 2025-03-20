using HSE_Bank.Application.Facades;
using HSE_Bank.Domain.Entities;
using HSE_Bank.Application.Commands;
using HSE_Bank.ConsoleApp.Utils;

namespace HSE_Bank.ConsoleApp.Menus
{
    public static class CategoryMenu
    {
        private static CategoryFacade _categoryFacade;

        public static void Init(CategoryFacade categoryFacade)
        {
            _categoryFacade = categoryFacade;
        }

        public static void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("====== Управление категориями ======");
                Console.WriteLine("1. Создать категорию");
                Console.WriteLine("2. Удалить категорию");
                Console.WriteLine("3. Показать все категории");
                Console.WriteLine("4. Вернуться в главное меню");

                int choice = ConsoleHelper.GetIntInput("Выберите действие (1-4): ", 1, 4);

                switch (choice)
                {
                    case 1:
                        CreateCategory();
                        break;
                    case 2:
                        DeleteCategory();
                        break;
                    case 3:
                        ShowAllCategories();
                        break;
                    case 4:
                        return;
                }
            }
        }

        private static void CreateCategory()
        {
            Console.Clear();
            Console.WriteLine("====== Создание новой категории ======");
            string name = ConsoleHelper.GetStringInput("Введите название категории: ");

            Console.WriteLine("Выберите тип категории:");
            Console.WriteLine("1. Доход");
            Console.WriteLine("2. Расход");
            int typeChoice = ConsoleHelper.GetIntInput("Введите номер (1-2): ", 1, 2);

            TypeCategory type = typeChoice == 1 ? TypeCategory.Income : TypeCategory.Expense;
            var categoryCreateCmd = new CreateCategoryCommand(_categoryFacade, name, type);
            categoryCreateCmd.Execute();
            Console.WriteLine($"Категория '{name}' ({type}) создана.");
            ConsoleHelper.WaitForKey();
        }

        private static void DeleteCategory()
        {
            Console.Clear();
            Console.WriteLine("====== Удаление категории ======");
            ShowAllCategories();

            int categoryId = ConsoleHelper.GetIntInput("Введите ID категории для удаления: ", 1, int.MaxValue);

            var category = _categoryFacade.GetCategoryById(categoryId);
            if (category == null)
            {
                Console.WriteLine("Ошибка: категория не найдена.");
            }
            else
            {
                var categoryDeleteCmd = new DeleteCategoryCommand(_categoryFacade, categoryId);
                categoryDeleteCmd.Execute();
                Console.WriteLine($"Категория '{category.Name}' удалена.");
            }

            ConsoleHelper.WaitForKey();
        }

        private static void ShowAllCategories()
        {
            Console.Clear();
            Console.WriteLine("====== Список категорий ======");
            var categories = _categoryFacade.GetAllCategories();

            if (categories == null || categories.Count() == 0)
            {
                Console.WriteLine("Нет доступных категорий.");
            }
            else
            {
                foreach (var category in categories)
                {
                    Console.WriteLine($"ID: {category.Id} | Название: {category.Name} | Тип: {category.Type}");
                }
            }

            ConsoleHelper.WaitForKey();
        }
    }
}
