using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using HSE_Bank.Domain.Entities;
using HSE_Bank.Infrastructure.Importers;
using Xunit;

namespace HSE_Bank_Tests;

public class DataImporterTests : IDisposable
{
    private readonly string _tempFolder;
    
    public DataImporterTests()
    {
        _tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempFolder);
    }
    
    [Fact]
    public void JsonDataImporter_ShouldImportValidData()
    {
        string filePath = Path.Combine(_tempFolder, "accounts.json");
        var jsonData = "[ {\"Id\":1,\"Name\":\"Test Account\",\"Balance\":100.0} ]";
        File.WriteAllText(filePath, jsonData);
        
        var importer = new JsonDataImporter<BankAccount>();
        var accounts = importer.Import(filePath);
        
        Assert.NotEmpty(accounts);
        Assert.Single(accounts);
        Assert.Equal(1, accounts.First().Id);
    }
    
    [Fact]
    public void JsonDataImporter_ShouldReturnEmptyList_WhenFileIsEmpty()
    {
        string filePath = Path.Combine(_tempFolder, "empty.json");
        File.WriteAllText(filePath, "");
        
        var importer = new JsonDataImporter<BankAccount>();
        var accounts = importer.Import(filePath);
        
        Assert.Empty(accounts);
    }
    
    [Fact]
    public void JsonDataImporter_ShouldReturnEmptyList_WhenJsonIsInvalid()
    {
        string filePath = Path.Combine(_tempFolder, "invalid.json");
        File.WriteAllText(filePath, "{invalid json}");
        
        var importer = new JsonDataImporter<BankAccount>();
        var accounts = importer.Import(filePath);
        
        Assert.Empty(accounts);
    }
    
    [Fact]
    public void JsonDataImporter_ShouldReturnEmptyList_WhenFileDoesNotExist()
    {
        var importer = new JsonDataImporter<BankAccount>();
        var accounts = importer.Import(Path.Combine(_tempFolder, "nonexistent.json"));
        
        Assert.Empty(accounts);
    }
    
    [Fact]
    public void CsvDataImporter_ShouldImportValidData()
    {
        string filePath = Path.Combine(_tempFolder, "accounts.csv");
        var csvData = "1,Test Account,100\n2,Another Account,200";
        File.WriteAllText(filePath, csvData);
        
        var importer = new CsvDataImporter<BankAccount>();
        var accounts = importer.Import(filePath);
        
        Assert.Equal(2, accounts.Count());
        Assert.Equal("Test Account", accounts.First().Name);
    }
    
    [Fact]
    public void CsvDataImporter_ShouldReturnEmptyList_WhenFileIsEmpty()
    {
        string filePath = Path.Combine(_tempFolder, "empty.csv");
        File.WriteAllText(filePath, "");
        
        var importer = new CsvDataImporter<BankAccount>();
        var accounts = importer.Import(filePath);
        
        Assert.Empty(accounts);
    }
    
    [Fact]
    public void CsvDataImporter_ShouldHandleDecimalParsingCorrectly()
    {
        string filePath = Path.Combine(_tempFolder, "accounts.csv");
        var csvData = "1,Test Account,10000\n2,Another Account,2050";
        File.WriteAllText(filePath, csvData);
        
        var importer = new CsvDataImporter<BankAccount>();
        var accounts = importer.Import(filePath);
        
        Assert.Equal(2, accounts.Count());
        Assert.Equal(10000, accounts.First().Balance);
        Assert.Equal(2050, accounts.Last().Balance);
    }
    
    public void Dispose()
    {
        try
        {
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
