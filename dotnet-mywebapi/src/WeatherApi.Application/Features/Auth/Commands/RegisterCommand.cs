using MediatR;
using WeatherApi.Application.DTOs;
using WeatherApi.Domain.Entities;
using WeatherApi.Domain.Enums;
using WeatherApi.Domain.Interfaces;

namespace WeatherApi.Application.Features.Auth.Commands;

public record RegisterCommand(string Username, string Password, string Email, string Role = "User") : IRequest<UserDto>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserDto>
{
    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"Username '{request.Username}' is already taken.");
        }

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out _))
        {
            throw new ArgumentException($"Invalid role '{request.Role}'. Valid roles are: {string.Join(", ", Enum.GetNames<UserRole>())}");
        }

        var user = new User
        {
            Username = request.Username,
            PasswordHash = ComputeHash(request.Password),
            Email = request.Email,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        var id = await _userRepository.CreateAsync(user);

        return new UserDto
        {
            Id = id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };
    }

    private static string ComputeHash(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hashBytes);
    }
}
