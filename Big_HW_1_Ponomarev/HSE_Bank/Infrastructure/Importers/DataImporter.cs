namespace HSE_Bank.Infrastructure.Importers;

public abstract class DataImporter<T>
{
    protected abstract string ReadFile(string filePath);
    protected abstract IEnumerable<T> ParseData(string data);
    protected abstract void SaveData(IEnumerable<T> data);
    public void Import(string filepath)
    {
        var data = ReadFile(filepath);
        var parsedData = ParseData(data);
        SaveData(parsedData);
    }
}