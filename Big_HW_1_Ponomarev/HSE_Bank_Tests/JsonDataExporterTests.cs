using System.Text.Json;
using HSE_Bank.Infrastructure.Exporters;
using HSE_Bank.Domain.Entities;

namespace HSE_Bank_Tests
{
    public class JsonDataExporterTests : IDisposable
    {
        private readonly string _tempFolder;
        private readonly string _exportFolder;
        private readonly string _accountFilePath;
        private readonly JsonDataExporter _exporter;

        public JsonDataExporterTests()
        {
            _tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _exportFolder = Path.Combine(_tempFolder, "Export");
            Directory.CreateDirectory(_exportFolder);
            Directory.SetCurrentDirectory(_tempFolder); 

            _exporter = new JsonDataExporter(); 
            _accountFilePath = Path.Combine(_exportFolder, "accounts.json");
        }

        [Fact]
        public void ExportBankAccount_CreatesFileWithCorrectJson()
        {
            if (File.Exists(_accountFilePath)) File.Delete(_accountFilePath);

            var account = new BankAccount { Id = 1, Name = "Test Account", Balance = 100.0m };
            _exporter.ExportBankAccount(account);

            int retries = 5;
            while (!File.Exists(_accountFilePath) && retries > 0)
            {
                Thread.Sleep(100);
                retries--;
            }

            Assert.True(File.Exists(_accountFilePath), "Файл JSON не был создан");

            string jsonContent = File.ReadAllText(_accountFilePath);
            var accounts = JsonSerializer.Deserialize<List<BankAccount>>(jsonContent);

            Assert.NotNull(accounts);
            Assert.Single(accounts);
            Assert.Equal(account.Id, accounts[0].Id);
            Assert.Equal(account.Name, accounts[0].Name);
            Assert.Equal(account.Balance, accounts[0].Balance);
        }

        public void Dispose()
        {
            try
            {
                Directory.SetCurrentDirectory(Path.GetTempPath()); 
                if (Directory.Exists(_tempFolder))
                {
                    Directory.Delete(_tempFolder, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cleaning up test files: {ex.Message}");
            }
        }
    }
}
