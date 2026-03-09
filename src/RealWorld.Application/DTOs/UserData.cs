namespace RealWorld.Application.DTOs;

public class UserData
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;

    public UserData() { }

    public UserData(string id, string email, string username, string bio, string image)
    {
        Id = id;
        Email = email;
        Username = username;
        Bio = bio;
        Image = image;
    }
}
