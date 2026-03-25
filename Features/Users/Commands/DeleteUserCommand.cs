using Dapper;
using MediatR;
using CqrsApi.Data;

namespace CqrsApi.Features.Users.Commands;

public record DeleteUserCommand(int Id) : IRequest<bool>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly DapperContext _context;

    public DeleteUserCommandHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM Users WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        var affected = await connection.ExecuteAsync(sql, new { request.Id });

        return affected > 0;
    }
}
