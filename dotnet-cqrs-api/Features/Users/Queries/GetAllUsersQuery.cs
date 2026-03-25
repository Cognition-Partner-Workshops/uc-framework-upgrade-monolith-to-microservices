using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Users.Queries;

public record GetAllUsersQuery : IRequest<IEnumerable<User>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<User>>
{
    private readonly DapperContext _context;

    public GetAllUsersQueryHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Users ORDER BY Id";

        using var connection = _context.CreateConnection();
        var users = await connection.QueryAsync<User>(sql);

        return users;
    }
}
