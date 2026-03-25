using Dapper;
using MediatR;
using CqrsApi.Data;

namespace CqrsApi.Features.Purchases.Commands;

public record DeletePurchaseCommand(int Id) : IRequest<bool>;

public class DeletePurchaseCommandHandler : IRequestHandler<DeletePurchaseCommand, bool>
{
    private readonly DapperContext _context;

    public DeletePurchaseCommandHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeletePurchaseCommand request, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM Purchases WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        var affected = await connection.ExecuteAsync(sql, new { request.Id });

        return affected > 0;
    }
}
