using MediatR;
using WeatherApi.Application.Common.Interfaces;
using WeatherApi.Application.DTOs;
using WeatherApi.Domain.Interfaces;

namespace WeatherApi.Application.Features.Auth.Commands;

public record LoginCommand(string Username, string Password) : IRequest<AuthResponse>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(IUserRepository userRepository, IJwtTokenService jwtTokenService, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            Username = user.Username,
            Role = user.Role
        };
    }
}
