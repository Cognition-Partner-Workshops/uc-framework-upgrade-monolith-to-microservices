using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Users.Queries;

public record GetUserByIdQuery(int Id) : IRequest<User?>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User?>
{
    private readonly DapperContext _context;

    public GetUserByIdQueryHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<User?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Users WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { request.Id });

        return user;
    }
}
