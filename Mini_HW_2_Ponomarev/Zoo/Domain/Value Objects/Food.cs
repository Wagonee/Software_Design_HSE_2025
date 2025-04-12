namespace Zoo.Domain.Value_Objects
{
    public sealed record Food
    {
        public string Name { get; }
        public Food(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Food name cannot be empty.", nameof(name));
            }
            Name = name;
        }
    }
}