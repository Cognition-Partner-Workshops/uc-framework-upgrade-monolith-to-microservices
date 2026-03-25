using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Purchases.Commands;

public record UpdatePurchaseCommand(
    int Id,
    int ProductId,
    int UserId,
    int Quantity,
    decimal UnitCost,
    string Supplier
) : IRequest<Purchase?>;

public class UpdatePurchaseCommandHandler : IRequestHandler<UpdatePurchaseCommand, Purchase?>
{
    private readonly DapperContext _context;

    public UpdatePurchaseCommandHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<Purchase?> Handle(UpdatePurchaseCommand request, CancellationToken cancellationToken)
    {
        var totalCost = request.Quantity * request.UnitCost;

        const string sql = @"
            UPDATE Purchases
            SET ProductId = @ProductId, UserId = @UserId, Quantity = @Quantity,
                UnitCost = @UnitCost, TotalCost = @TotalCost, Supplier = @Supplier
            WHERE Id = @Id;
            SELECT * FROM Purchases WHERE Id = @Id;";

        using var connection = _context.CreateConnection();
        var purchase = await connection.QuerySingleOrDefaultAsync<Purchase>(sql, new
        {
            request.Id,
            request.ProductId,
            request.UserId,
            request.Quantity,
            request.UnitCost,
            TotalCost = totalCost,
            request.Supplier
        });

        return purchase;
    }
}
