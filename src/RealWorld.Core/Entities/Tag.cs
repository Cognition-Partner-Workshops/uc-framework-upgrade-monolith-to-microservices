namespace RealWorld.Core.Entities;

public class Tag
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public Tag() { }

    public Tag(string name)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
    }

    public override bool Equals(object? obj) => obj is Tag t && t.Name == Name;
    public override int GetHashCode() => Name.GetHashCode();
}
