using System.ComponentModel.DataAnnotations;

namespace RealWorld.Api.DTOs;

// --- Request DTOs ---

public class RegisterUserRequest
{
    public RegisterUserDto User { get; set; } = new();
}

public class RegisterUserDto
{
    [Required(ErrorMessage = "can't be empty")]
    [EmailAddress(ErrorMessage = "should be an email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "can't be empty")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "can't be empty")]
    public string Password { get; set; } = string.Empty;
}

public class LoginUserRequest
{
    public LoginUserDto User { get; set; } = new();
}

public class LoginUserDto
{
    [Required(ErrorMessage = "can't be empty")]
    [EmailAddress(ErrorMessage = "should be an email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "can't be empty")]
    public string Password { get; set; } = string.Empty;
}

public class UpdateUserRequest
{
    public UpdateUserDto User { get; set; } = new();
}

public class UpdateUserDto
{
    [EmailAddress(ErrorMessage = "should be an email")]
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Bio { get; set; }
    public string? Image { get; set; }
}

// --- Response DTOs ---

public class UserResponse
{
    public UserDataDto User { get; set; } = new();
}

public class UserDataDto
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Image { get; set; }
    public string Token { get; set; } = string.Empty;
}
