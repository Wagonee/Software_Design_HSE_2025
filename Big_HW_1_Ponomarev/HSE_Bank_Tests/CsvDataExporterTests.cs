using System.Text;
using HSE_Bank.Infrastructure.Exporters;
using HSE_Bank.Domain.Entities;

namespace HSE_Bank_Tests;

public class CsvDataExporterTests : IDisposable
{
    private readonly string _tempFolder;
    private readonly string _exportFolder;
    private readonly string _accountFilePath;

    public CsvDataExporterTests()
    {
        _tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _exportFolder = Path.Combine(_tempFolder, "Export");
        Directory.CreateDirectory(_exportFolder);
        Directory.SetCurrentDirectory(_tempFolder);

        _accountFilePath = Path.Combine(_exportFolder, "accounts.csv");
    }

    [Fact]
    public void ExportBankAccount_AppendsCorrectCsvLine()
    {
        if (File.Exists(_accountFilePath))
        {
            File.Delete(_accountFilePath); 
        }
            
        var exporter = new CsvDataExporter();
        var account = new BankAccount { Id = 2, Name = "CSV Account", Balance = 2500 };
        exporter.ExportBankAccount(account);
 
        int retries = 5;
        while (!File.Exists(_accountFilePath) && retries > 0)
        {
            Thread.Sleep(100);
            retries--;
        }
            
        Assert.True(File.Exists(_accountFilePath), "Файл CSV не был создан");
            
        string[] lines = File.ReadAllLines(_accountFilePath, Encoding.UTF8);
        Assert.Single(lines);
        string expectedLine = $"{account.Id},{account.Name},{account.Balance}";
        Assert.Equal(expectedLine, lines.First().Trim());
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
        catch (Exception ex) { Console.WriteLine($"Cleanup error: {ex.Message}"); }
    }
}