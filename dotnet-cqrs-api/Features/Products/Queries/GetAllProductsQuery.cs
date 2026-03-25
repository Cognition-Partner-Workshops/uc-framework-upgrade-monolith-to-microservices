using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Products.Queries;

public record GetAllProductsQuery : IRequest<IEnumerable<Product>>;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<Product>>
{
    private readonly DapperContext _context;

    public GetAllProductsQueryHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Products ORDER BY Id";

        using var connection = _context.CreateConnection();
        var products = await connection.QueryAsync<Product>(sql);

        return products;
    }
}
