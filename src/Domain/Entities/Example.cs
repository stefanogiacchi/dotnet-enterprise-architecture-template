namespace Domain.Entities;

public class Example
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private Example() { }

    public Example(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be empty.", nameof(newName));

        Name = newName;
    }
}
