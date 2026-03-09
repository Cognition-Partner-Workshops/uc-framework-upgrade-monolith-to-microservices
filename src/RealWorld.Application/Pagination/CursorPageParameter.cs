namespace RealWorld.Application.Pagination;

public class CursorPageParameter<T>
{
    private const int MaxLimit = 1000;

    public int Limit { get; private set; } = 20;
    public T? Cursor { get; private set; }
    public Direction Direction { get; private set; }

    public CursorPageParameter() { }

    public CursorPageParameter(T? cursor, int limit, Direction direction)
    {
        SetLimit(limit);
        Cursor = cursor;
        Direction = direction;
    }

    public bool IsNext() => Direction == Direction.Next;

    public int QueryLimit => Limit + 1;

    private void SetLimit(int limit)
    {
        if (limit > MaxLimit)
            Limit = MaxLimit;
        else if (limit > 0)
            Limit = limit;
    }
}
