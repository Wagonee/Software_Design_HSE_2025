namespace HSE_Bank.Infrastructure.Importers;

public abstract class DataImporter<T>
{
    protected abstract string ReadFile(string filePath);
    protected abstract IEnumerable<T> ParseData(string data);
    public abstract IEnumerable<T> Import(string filepath);
}