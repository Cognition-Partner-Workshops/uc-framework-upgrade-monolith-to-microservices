using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Purchases.Queries;

public record GetAllPurchasesQuery : IRequest<IEnumerable<Purchase>>;

public class GetAllPurchasesQueryHandler : IRequestHandler<GetAllPurchasesQuery, IEnumerable<Purchase>>
{
    private readonly DapperContext _context;

    public GetAllPurchasesQueryHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Purchase>> Handle(GetAllPurchasesQuery request, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Purchases ORDER BY Id";

        using var connection = _context.CreateConnection();
        var purchases = await connection.QueryAsync<Purchase>(sql);

        return purchases;
    }
}
