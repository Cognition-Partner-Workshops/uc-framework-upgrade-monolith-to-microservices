using RealWorld.Core.Entities;
using RealWorld.Core.Interfaces;

namespace RealWorld.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly string _defaultImage;

    public UserService(IUserRepository userRepository, string defaultImage)
    {
        _userRepository = userRepository;
        _defaultImage = defaultImage;
    }

    public async Task<User> CreateUserAsync(string email, string username, string hashedPassword)
    {
        // Check for duplicate email
        var existingByEmail = await _userRepository.FindByEmailAsync(email);
        if (existingByEmail != null)
            throw new InvalidOperationException("email already exist");

        // Check for duplicate username
        var existingByUsername = await _userRepository.FindByUsernameAsync(username);
        if (existingByUsername != null)
            throw new InvalidOperationException("username already exist");

        var user = new User(email, username, hashedPassword, "", _defaultImage);
        await _userRepository.SaveAsync(user);
        return user;
    }

    public async Task UpdateUserAsync(User targetUser, string? email, string? username, string? password, string? bio, string? image)
    {
        // Validate email uniqueness
        if (!string.IsNullOrWhiteSpace(email))
        {
            var existingByEmail = await _userRepository.FindByEmailAsync(email);
            if (existingByEmail != null && existingByEmail.Id != targetUser.Id)
                throw new InvalidOperationException("email already exist");
        }

        // Validate username uniqueness
        if (!string.IsNullOrWhiteSpace(username))
        {
            var existingByUsername = await _userRepository.FindByUsernameAsync(username);
            if (existingByUsername != null && existingByUsername.Id != targetUser.Id)
                throw new InvalidOperationException("username already exist");
        }

        targetUser.Update(email, username, password, bio, image);
        await _userRepository.SaveAsync(targetUser);
    }
}
