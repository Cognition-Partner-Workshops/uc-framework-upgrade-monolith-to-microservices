namespace RealWorld.Api.DTOs;

public class ProfileResponse
{
    public ProfileDataDto Profile { get; set; } = new();
}

public class ProfileDataDto
{
    public string Username { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Image { get; set; }
    public bool Following { get; set; }
}
