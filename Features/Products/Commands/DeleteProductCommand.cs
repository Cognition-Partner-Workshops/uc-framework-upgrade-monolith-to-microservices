using Dapper;
using MediatR;
using CqrsApi.Data;

namespace CqrsApi.Features.Products.Commands;

public record DeleteProductCommand(int Id) : IRequest<bool>;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly DapperContext _context;

    public DeleteProductCommandHandler(DapperContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM Products WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        var affected = await connection.ExecuteAsync(sql, new { request.Id });

        return affected > 0;
    }
}
