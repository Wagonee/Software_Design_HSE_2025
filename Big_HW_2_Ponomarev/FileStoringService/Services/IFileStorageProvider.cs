namespace FileStoringService.Services
{
    public interface IFileStorageProvider
    {
        Task<string> SaveFileAsync(Stream stream, string originalFileName, string contentType);
        Task<Stream> GetFileAsync(string storedFileName);
        Task<bool> DeleteFileAsync(string storedFileName);
        string GetFilePath(string storedFileName); 
    }
}