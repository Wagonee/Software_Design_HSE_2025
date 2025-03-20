using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Repositories;
using HSE_Bank.Infrastructure.Exporters;
using HSE_Bank.Infrastructure.Importers;
using HSE_Bank.ConsoleApp.Utils;

namespace HSE_Bank.ConsoleApp.Menus
{
    public static class ImportExportMenu
    {
        private static IAccountRepository _accountRepository;
        private static ICategoryRepository _categoryRepository;
        private static IOperationRepository _operationRepository;
        
        private static readonly string ExportFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Export");
        private static readonly string AccountCsvFilePath = Path.Combine(ExportFolderPath, "accounts.csv");
        private static readonly string CategoryCsvFilePath = Path.Combine(ExportFolderPath, "categories.csv");
        private static readonly string OperationCsvFilePath = Path.Combine(ExportFolderPath, "operations.csv");

        private static readonly string AccountJsonFilePath = Path.Combine(ExportFolderPath, "accounts.json");
        private static readonly string CategoryJsonFilePath = Path.Combine(ExportFolderPath, "categories.json");
        private static readonly string OperationJsonFilePath = Path.Combine(ExportFolderPath, "operations.json");

        public static void Init(
            IAccountRepository accountRepository,
            ICategoryRepository categoryRepository,
            IOperationRepository operationRepository
        )
        {
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository;
            _operationRepository = operationRepository;
        }

        public static void Show()
        {
            if (_accountRepository == null || _categoryRepository == null || _operationRepository == null)
            {
                Console.WriteLine("Репозитории не инициализированы. Сначала инициализируйте, затем повторите попытку.");
                ConsoleHelper.WaitForKey();
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("====== Экспорт и импорт данных ======");
                Console.WriteLine("1. Экспорт данных в CSV");
                Console.WriteLine("2. Экспорт данных в JSON");
                Console.WriteLine("3. Импорт данных из CSV");
                Console.WriteLine("4. Импорт данных из JSON");
                Console.WriteLine("0. Назад");
                
                int choice = ConsoleHelper.GetIntInput("Выберите действие (0-4): ", 0, 4);

                switch (choice)
                {
                    case 1:
                        ExportDataToCsv();
                        break;
                    case 2:
                        ExportDataToJson();
                        break;
                    case 3:
                        ImportDataFromCsv();
                        break;
                    case 4:
                        ImportDataFromJson();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Неверный ввод. Попробуйте снова.");
                        ConsoleHelper.WaitForKey();
                        break;
                }
            }
        }

        private static void ExportDataToCsv()
        {
            Console.Clear();
            try
            {
                var exporter = new CsvDataExporter();

                var accounts = _accountRepository.GetAllAccounts();
                foreach (var account in accounts)
                {
                    exporter.ExportBankAccount(account);
                }

                var categories = _categoryRepository.GetAllCategories();
                foreach (var cat in categories)
                {
                    exporter.ExportCategory(cat);
                }

                var operations = _operationRepository.GetAllOperations();
                foreach (var op in operations)
                {
                    exporter.ExportOperation(op);
                }

                Console.WriteLine("Данные успешно экспортированы в CSV.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при экспорте в CSV: {ex.Message}");
            }
            ConsoleHelper.WaitForKey();
        }

        private static void ExportDataToJson()
        {
            Console.Clear();
            try
            {
                var exporter = new JsonDataExporter();

                var accounts = _accountRepository.GetAllAccounts();
                foreach (var account in accounts)
                {
                    exporter.ExportBankAccount(account);
                }

                var categories = _categoryRepository.GetAllCategories();
                foreach (var cat in categories)
                {
                    exporter.ExportCategory(cat);
                }

                var operations = _operationRepository.GetAllOperations();
                foreach (var op in operations)
                {
                    exporter.ExportOperation(op);
                }

                Console.WriteLine("Данные успешно экспортированы в JSON.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при экспорте в JSON: {ex.Message}");
            }
            ConsoleHelper.WaitForKey();
        }

        private static void ImportDataFromCsv()
        {
            Console.Clear();
            try
            {
                if (!Directory.Exists(ExportFolderPath))
                {
                    Console.WriteLine("Папка экспорта не найдена.");
                    ConsoleHelper.WaitForKey();
                    return;
                }

                var accountImporter = new CsvDataImporter<BankAccount>();
                var importedAccounts = accountImporter.Import(AccountCsvFilePath);
                foreach (var acc in importedAccounts)
                    _accountRepository.AddAccount(acc);

                var categoryImporter = new CsvDataImporter<Category>();
                var importedCategories = categoryImporter.Import(CategoryCsvFilePath);
                foreach (var cat in importedCategories)
                    _categoryRepository.AddCategory(cat);

                var operationImporter = new CsvDataImporter<Operation>();
                var importedOperations = operationImporter.Import(OperationCsvFilePath);
                foreach (var op in importedOperations)
                    _operationRepository.AddOperation(op);

                Console.WriteLine("Данные успешно импортированы из CSV.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при импорте из CSV: {ex.Message}");
            }
            ConsoleHelper.WaitForKey();
        }

        private static void ImportDataFromJson()
        {
            Console.Clear();
            try
            {
                if (!Directory.Exists(ExportFolderPath))
                {
                    Console.WriteLine("Папка экспорта не найдена.");
                    ConsoleHelper.WaitForKey();
                    return;
                }

                var accountImporter = new JsonDataImporter<BankAccount>();
                var importedAccounts = accountImporter.Import(AccountJsonFilePath);
                foreach (var acc in importedAccounts)
                    _accountRepository.AddAccount(acc);

                var categoryImporter = new JsonDataImporter<Category>();
                var importedCategories = categoryImporter.Import(CategoryJsonFilePath);
                foreach (var cat in importedCategories)
                    _categoryRepository.AddCategory(cat);

                var operationImporter = new JsonDataImporter<Operation>();
                var importedOperations = operationImporter.Import(OperationJsonFilePath);
                foreach (var op in importedOperations)
                    _operationRepository.AddOperation(op);

                Console.WriteLine("Данные успешно импортированы из JSON.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при импорте из JSON: {ex.Message}");
            }
            ConsoleHelper.WaitForKey();
        }
    }
}
