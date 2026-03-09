namespace RealWorld.Application.DTOs;

public class UserWithToken
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string Bio { get; set; }
    public string Image { get; set; }
    public string Token { get; set; }

    public UserWithToken(UserData userData, string token)
    {
        Email = userData.Email;
        Username = userData.Username;
        Bio = userData.Bio;
        Image = userData.Image;
        Token = token;
    }
}
