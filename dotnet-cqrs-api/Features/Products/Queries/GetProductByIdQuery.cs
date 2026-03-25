using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Products.Queries;

public record GetProductByIdQuery(int Id) : IRequest<Product?>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product?>
{
    private readonly DapperContext _context;

    public GetProductByIdQueryHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Products WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        var product = await connection.QuerySingleOrDefaultAsync<Product>(sql, new { request.Id });

        return product;
    }
}
