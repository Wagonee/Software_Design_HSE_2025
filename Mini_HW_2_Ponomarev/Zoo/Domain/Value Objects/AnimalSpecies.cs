namespace Zoo.Domain.Value_Objects;

public sealed class AnimalSpecies
{
    public string Type { get; }
    public bool IsPredator { get; }

    public AnimalSpecies(string type, bool isPredator)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException($"'{nameof(type)}' cannot be null or whitespace.", nameof(type));
        }
        Type = type;
        IsPredator = isPredator;
    }
}