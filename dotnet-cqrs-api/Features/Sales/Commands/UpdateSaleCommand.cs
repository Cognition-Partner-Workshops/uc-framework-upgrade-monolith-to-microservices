using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Sales.Commands;

public record UpdateSaleCommand(
    int Id,
    int ProductId,
    int UserId,
    int Quantity,
    decimal UnitPrice
) : IRequest<Sale?>;

public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, Sale?>
{
    private readonly DapperContext _context;

    public UpdateSaleCommandHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<Sale?> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var totalAmount = request.Quantity * request.UnitPrice;

        const string sql = @"
            UPDATE Sales
            SET ProductId = @ProductId, UserId = @UserId, Quantity = @Quantity,
                UnitPrice = @UnitPrice, TotalAmount = @TotalAmount
            WHERE Id = @Id;
            SELECT * FROM Sales WHERE Id = @Id;";

        using var connection = _context.CreateConnection();
        var sale = await connection.QuerySingleOrDefaultAsync<Sale>(sql, new
        {
            request.Id,
            request.ProductId,
            request.UserId,
            request.Quantity,
            request.UnitPrice,
            TotalAmount = totalAmount
        });

        return sale;
    }
}
