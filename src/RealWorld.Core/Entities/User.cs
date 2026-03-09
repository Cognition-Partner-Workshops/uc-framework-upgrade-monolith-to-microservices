namespace RealWorld.Core.Entities;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;

    public User() { }

    public User(string email, string username, string password, string bio, string image)
    {
        Id = Guid.NewGuid().ToString();
        Email = email;
        Username = username;
        Password = password;
        Bio = bio;
        Image = image;
    }

    public void Update(string? email, string? username, string? password, string? bio, string? image)
    {
        if (!string.IsNullOrWhiteSpace(email))
            Email = email;
        if (!string.IsNullOrWhiteSpace(username))
            Username = username;
        if (!string.IsNullOrWhiteSpace(password))
            Password = password;
        if (!string.IsNullOrWhiteSpace(bio))
            Bio = bio;
        if (!string.IsNullOrWhiteSpace(image))
            Image = image;
    }

    public override bool Equals(object? obj) => obj is User u && u.Id == Id;
    public override int GetHashCode() => Id.GetHashCode();
}
