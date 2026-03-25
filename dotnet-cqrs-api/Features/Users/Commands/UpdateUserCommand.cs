using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Users.Commands;

public record UpdateUserCommand(
    int Id,
    string Username,
    string Email,
    string FullName,
    string Role
) : IRequest<User?>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User?>
{
    private readonly DapperContext _context;

    public UpdateUserCommandHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<User?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        const string sql = @"
            UPDATE Users
            SET Username = @Username, Email = @Email, FullName = @FullName,
                Role = @Role, UpdatedAt = datetime('now')
            WHERE Id = @Id;
            SELECT * FROM Users WHERE Id = @Id;";

        using var connection = _context.CreateConnection();
        var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new
        {
            request.Id,
            request.Username,
            request.Email,
            request.FullName,
            request.Role
        });

        return user;
    }
}
