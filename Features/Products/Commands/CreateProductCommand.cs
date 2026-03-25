using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Products.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity
) : IRequest<Product>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly DapperContext _context;

    public CreateProductCommandHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO Products (Name, Description, Price, StockQuantity, CreatedAt, UpdatedAt)
            VALUES (@Name, @Description, @Price, @StockQuantity, datetime('now'), datetime('now'));
            SELECT * FROM Products WHERE Id = last_insert_rowid();";

        using var connection = _context.CreateConnection();
        var product = await connection.QuerySingleAsync<Product>(sql, new
        {
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity
        });

        return product;
    }
}
