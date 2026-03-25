using Dapper;
using MediatR;
using CqrsApi.Data;

namespace CqrsApi.Features.Sales.Commands;

public record DeleteSaleCommand(int Id) : IRequest<bool>;

public class DeleteSaleCommandHandler : IRequestHandler<DeleteSaleCommand, bool>
{
    private readonly DapperContext _context;

    public DeleteSaleCommandHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM Sales WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        var affected = await connection.ExecuteAsync(sql, new { request.Id });

        return affected > 0;
    }
}
