namespace RealWorld.Api.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;

    public List<ArticleFavorite> Favorites { get; set; } = new();
    public List<FollowRelation> Following { get; set; } = new();
    public List<FollowRelation> Followers { get; set; } = new();
    public List<Article> Articles { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();

    public void Update(string? email, string? username, string? password, string? bio, string? image)
    {
        if (!string.IsNullOrEmpty(email)) Email = email;
        if (!string.IsNullOrEmpty(username)) Username = username;
        if (!string.IsNullOrEmpty(password)) Password = password;
        if (!string.IsNullOrEmpty(bio)) Bio = bio;
        if (!string.IsNullOrEmpty(image)) Image = image;
    }
}
