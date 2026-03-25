using Dapper;
using MediatR;
using CqrsApi.Data;
using CqrsApi.Models;

namespace CqrsApi.Features.Purchases.Commands;

public record CreatePurchaseCommand(
    int ProductId,
    int UserId,
    int Quantity,
    decimal UnitCost,
    string Supplier
) : IRequest<Purchase>;

public class CreatePurchaseCommandHandler : IRequestHandler<CreatePurchaseCommand, Purchase>
{
    private readonly DapperContext _context;

    public CreatePurchaseCommandHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<Purchase> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        var totalCost = request.Quantity * request.UnitCost;

        const string sql = @"
            INSERT INTO Purchases (ProductId, UserId, Quantity, UnitCost, TotalCost, Supplier, PurchaseDate, CreatedAt)
            VALUES (@ProductId, @UserId, @Quantity, @UnitCost, @TotalCost, @Supplier, datetime('now'), datetime('now'));
            SELECT * FROM Purchases WHERE Id = last_insert_rowid();";

        using var connection = _context.CreateConnection();
        var purchase = await connection.QuerySingleAsync<Purchase>(sql, new
        {
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
