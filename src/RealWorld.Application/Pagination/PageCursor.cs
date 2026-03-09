namespace RealWorld.Application.Pagination;

public abstract class PageCursor<T>
{
    public T Data { get; }

    protected PageCursor(T data)
    {
        Data = data;
    }

    public override string ToString()
    {
        return Data?.ToString() ?? string.Empty;
    }
}
