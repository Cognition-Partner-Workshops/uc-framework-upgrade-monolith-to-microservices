using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Purchases.Queries;

public record GetPurchaseByIdQuery(int Id) : IRequest<Purchase?>;

public class GetPurchaseByIdQueryHandler : IRequestHandler<GetPurchaseByIdQuery, Purchase?>
{
    private readonly DapperContext _context;

    public GetPurchaseByIdQueryHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<Purchase?> Handle(GetPurchaseByIdQuery request, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Purchases WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        var purchase = await connection.QuerySingleOrDefaultAsync<Purchase>(sql, new { request.Id });

        return purchase;
    }
}
