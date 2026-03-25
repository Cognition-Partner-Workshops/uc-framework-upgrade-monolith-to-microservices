using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Products.Commands;

public record UpdateProductCommand(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity
) : IRequest<Product?>;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Product?>
{
    private readonly DapperContext _context;

    public UpdateProductCommandHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<Product?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        const string sql = @"
            UPDATE Products
            SET Name = @Name, Description = @Description, Price = @Price,
                StockQuantity = @StockQuantity, UpdatedAt = datetime('now')
            WHERE Id = @Id;
            SELECT * FROM Products WHERE Id = @Id;";

        using var connection = _context.CreateConnection();
        var product = await connection.QuerySingleOrDefaultAsync<Product>(sql, new
        {
            request.Id,
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity
        });

        return product;
    }
}
