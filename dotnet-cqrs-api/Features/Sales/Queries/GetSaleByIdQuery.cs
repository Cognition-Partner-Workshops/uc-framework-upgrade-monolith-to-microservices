using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Sales.Queries;

public record GetSaleByIdQuery(int Id) : IRequest<Sale?>;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, Sale?>
{
    private readonly DapperContext _context;

    public GetSaleByIdQueryHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<Sale?> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Sales WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        var sale = await connection.QuerySingleOrDefaultAsync<Sale>(sql, new { request.Id });

        return sale;
    }
}
