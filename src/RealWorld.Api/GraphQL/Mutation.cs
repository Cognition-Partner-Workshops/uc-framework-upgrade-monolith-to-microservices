using RealWorld.Application.DTOs;
using RealWorld.Application.Services;
using RealWorld.Core.Interfaces;

namespace RealWorld.Api.GraphQL;

public class Mutation
{
    public async Task<UserWithToken> CreateUser(
        [Service] UserService userService,
        [Service] UserQueryService userQueryService,
        [Service] IJwtService jwtService,
        string email,
        string username,
        string password)
    {
        var user = await userService.CreateUserAsync(email, username, BCrypt.Net.BCrypt.HashPassword(password));
        var userData = await userQueryService.FindByIdAsync(user.Id);
        var token = jwtService.ToToken(user);
        return new UserWithToken(userData!, token);
    }

    public async Task<UserWithToken> Login(
        [Service] IUserRepository userRepository,
        [Service] UserQueryService userQueryService,
        [Service] IJwtService jwtService,
        string email,
        string password)
    {
        var user = await userRepository.FindByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            throw new GraphQLException("invalid email or password");

        var userData = await userQueryService.FindByIdAsync(user.Id);
        var token = jwtService.ToToken(user);
        return new UserWithToken(userData!, token);
    }
}
