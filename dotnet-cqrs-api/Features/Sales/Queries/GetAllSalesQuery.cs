using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Sales.Queries;

public record GetAllSalesQuery : IRequest<IEnumerable<Sale>>;

public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, IEnumerable<Sale>>
{
    private readonly DapperContext _context;

    public GetAllSalesQueryHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Sale>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Sales ORDER BY Id";

        using var connection = _context.CreateConnection();
        var sales = await connection.QueryAsync<Sale>(sql);

        return sales;
    }
}
