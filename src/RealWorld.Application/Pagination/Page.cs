namespace RealWorld.Application.Pagination;

public class Page
{
    private const int MaxLimit = 100;

    public int Offset { get; private set; }
    public int Limit { get; private set; } = 20;

    public Page() { }

    public Page(int offset, int limit)
    {
        SetOffset(offset);
        SetLimit(limit);
    }

    private void SetOffset(int offset)
    {
        if (offset > 0)
            Offset = offset;
    }

    private void SetLimit(int limit)
    {
        if (limit > MaxLimit)
            Limit = MaxLimit;
        else if (limit > 0)
            Limit = limit;
    }
}
