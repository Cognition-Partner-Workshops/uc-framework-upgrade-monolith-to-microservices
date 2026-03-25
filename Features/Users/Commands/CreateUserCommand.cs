using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Users.Commands;

public record CreateUserCommand(
    string Username,
    string Email,
    string FullName,
    string Role
) : IRequest<User>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly DapperContext _context;

    public CreateUserCommandHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO Users (Username, Email, FullName, Role, CreatedAt, UpdatedAt)
            VALUES (@Username, @Email, @FullName, @Role, datetime('now'), datetime('now'));
            SELECT * FROM Users WHERE Id = last_insert_rowid();";

        using var connection = _context.CreateConnection();
        var user = await connection.QuerySingleAsync<User>(sql, new
        {
            request.Username,
            request.Email,
            request.FullName,
            request.Role
        });

        return user;
    }
}
