using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Sales.Commands;

public record CreateSaleCommand(
    int ProductId,
    int UserId,
    int Quantity,
    decimal UnitPrice
) : IRequest<Sale>;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Sale>
{
    private readonly DapperContext _context;

    public CreateSaleCommandHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<Sale> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var totalAmount = request.Quantity * request.UnitPrice;

        const string sql = @"
            INSERT INTO Sales (ProductId, UserId, Quantity, UnitPrice, TotalAmount, SaleDate, CreatedAt)
            VALUES (@ProductId, @UserId, @Quantity, @UnitPrice, @TotalAmount, datetime('now'), datetime('now'));
            SELECT * FROM Sales WHERE Id = last_insert_rowid();";

        using var connection = _context.CreateConnection();
        var sale = await connection.QuerySingleAsync<Sale>(sql, new
        {
            request.ProductId,
            request.UserId,
            request.Quantity,
            request.UnitPrice,
            TotalAmount = totalAmount
        });

        return sale;
    }
}
