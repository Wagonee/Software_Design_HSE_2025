using Moq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using FileStoringService.Services;
using System.Text;

namespace FileStoringService.Tests;

public class LocalFileStorageProviderTests : IDisposable
{
    private readonly Mock<ILogger<LocalFileStorageProvider>> _mockLogger;
    private readonly string _baseTestPath;

    public LocalFileStorageProviderTests()
    {
        _mockLogger = new Mock<ILogger<LocalFileStorageProvider>>();
        _baseTestPath = Path.Combine(Path.GetTempPath(), "FileStorageTest_" + Guid.NewGuid().ToString());
        if (Directory.Exists(_baseTestPath))
        {
            Directory.Delete(_baseTestPath, true);
        }
    }

    private IOptions<FileStorageSettings> CreateSettings(string basePath)
    {
        var settings = new FileStorageSettings { BasePath = basePath };
        return Options.Create(settings);
    }

    [Fact]
    public void Constructor_ValidPath_CreatesDirectoryIfNotExists()
    {

        var settings = CreateSettings(_baseTestPath);

      
        var provider = new LocalFileStorageProvider(settings, _mockLogger.Object);


        Assert.True(Directory.Exists(_baseTestPath));
    }
    
    [Fact]
    public void Constructor_BasePathAlreadyExists_DoesNotThrow()
    {
 
        Directory.CreateDirectory(_baseTestPath); 
        var settings = CreateSettings(_baseTestPath);

        var exception = Record.Exception(() => new LocalFileStorageProvider(settings, _mockLogger.Object));
        Assert.Null(exception);
        Assert.True(Directory.Exists(_baseTestPath));
    }

    [Fact]
    public void Constructor_NullBasePath_ThrowsArgumentNullException()
    {

        var settings = CreateSettings(null);

    
        var ex = Assert.Throws<ArgumentNullException>(() => new LocalFileStorageProvider(settings, _mockLogger.Object));
        Assert.Contains("File storage base path is not configured.", ex.Message);
    }
    
    [Fact]
    public void Constructor_DirectoryCreationFails_ThrowsAndLogsError()
    {

        var invalidPath = Path.Combine(Path.GetTempPath(), "invalid:path");
         if (Directory.Exists(invalidPath))
        {
            Directory.Delete(invalidPath, true); 
        }
        var settings = CreateSettings(invalidPath);
        var mockLoggerForError = new Mock<ILogger<LocalFileStorageProvider>>();

        var ex = Assert.ThrowsAny<Exception>(() => new LocalFileStorageProvider(settings, mockLoggerForError.Object));
         mockLoggerForError.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to create storage directory")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.AtLeastOnce); 

        if (Directory.Exists(invalidPath))
        {
            Directory.Delete(invalidPath, true);
        }
    }


    [Fact]
    public async Task SaveFileAsync_ValidStream_SavesFileAndReturnsName()
    {

        var settings = CreateSettings(_baseTestPath);
        var provider = new LocalFileStorageProvider(settings, _mockLogger.Object);
        var originalFileName = "mytest.txt";
        var fileContent = "Hello, Xunit!";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

  
        var storedFileName = await provider.SaveFileAsync(stream, originalFileName, "text/plain");


        Assert.NotNull(storedFileName);
        Assert.EndsWith(Path.GetExtension(originalFileName), storedFileName);
        var filePath = Path.Combine(_baseTestPath, storedFileName);
        Assert.True(File.Exists(filePath));
        var savedContent = await File.ReadAllTextAsync(filePath);
        Assert.Equal(fileContent, savedContent);
    }
    
    [Fact]
    public async Task SaveFileAsync_StreamPositionNotZero_ResetsAndSavesFile()
    {
        var settings = CreateSettings(_baseTestPath);
        var provider = new LocalFileStorageProvider(settings, _mockLogger.Object);
        var originalFileName = "positioned.txt";
        var fileContent = "Stream position test";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        stream.Position = 5;

       
        var storedFileName = await provider.SaveFileAsync(stream, originalFileName, "text/plain");

     
        Assert.NotNull(storedFileName);
        var filePath = Path.Combine(_baseTestPath, storedFileName);
        Assert.True(File.Exists(filePath));
        var savedContent = await File.ReadAllTextAsync(filePath);
        Assert.Equal(fileContent, savedContent);
    }

    [Fact]
    public async Task GetFileAsync_ExistingFile_ReturnsFileStream()
    {
       
        var settings = CreateSettings(_baseTestPath);
        var provider = new LocalFileStorageProvider(settings, _mockLogger.Object);
        var originalFileName = "retrievable.txt";
        var fileContent = "Content to retrieve.";
        var streamToSave = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        var storedFileName = await provider.SaveFileAsync(streamToSave, originalFileName, "text/plain");

      
        using (var retrievedStream = await provider.GetFileAsync(storedFileName))
        using (var reader = new StreamReader(retrievedStream))
        {
            var retrievedContent = await reader.ReadToEndAsync();
            Assert.Equal(fileContent, retrievedContent);
        }
    }

    [Fact]
    public async Task GetFileAsync_NonExistingFile_ThrowsFileNotFoundException()
    {
    
        var settings = CreateSettings(_baseTestPath); 
        var provider = new LocalFileStorageProvider(settings, _mockLogger.Object);
        var nonExistentFileName = "ghost.txt";

        var ex = await Assert.ThrowsAsync<FileNotFoundException>(() => provider.GetFileAsync(nonExistentFileName));
        Assert.Contains("File not found in storage.", ex.Message);
        Assert.Equal(nonExistentFileName, ex.FileName);
    }

    [Fact]
    public async Task DeleteFileAsync_ExistingFile_DeletesFileAndReturnsTrue()
    {

        var settings = CreateSettings(_baseTestPath);
        var provider = new LocalFileStorageProvider(settings, _mockLogger.Object);
        var originalFileName = "deletable.txt";
        var fileContent = "Content to delete.";
        var streamToSave = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        var storedFileName = await provider.SaveFileAsync(streamToSave, originalFileName, "text/plain");
        var filePath = Path.Combine(_baseTestPath, storedFileName);
        Assert.True(File.Exists(filePath)); 


        var result = await provider.DeleteFileAsync(storedFileName);

  
        Assert.True(result);
        Assert.False(File.Exists(filePath));
    }

    [Fact]
    public async Task DeleteFileAsync_NonExistingFile_ReturnsFalse()
    { 
        var settings = CreateSettings(_baseTestPath);
        var provider = new LocalFileStorageProvider(settings, _mockLogger.Object);
        var nonExistentFileName = "never_existed.txt";


        var result = await provider.DeleteFileAsync(nonExistentFileName);


        Assert.False(result);
    }
    
    


    [Fact]
    public void GetFilePath_ReturnsCorrectPath()
    {
        var settings = CreateSettings(_baseTestPath);
        var provider = new LocalFileStorageProvider(settings, _mockLogger.Object);
        var storedFileName = "test_file.dat";

        var resultPath = provider.GetFilePath(storedFileName);

        Assert.Equal(Path.Combine(_baseTestPath, storedFileName), resultPath);
    }


    public void Dispose()
    {
        if (Directory.Exists(_baseTestPath))
        {
            Directory.Delete(_baseTestPath, true);
        }
    }
}