namespace RealWorld.Application.Pagination;

public class CursorPager<T> where T : INode
{
    public List<T> Data { get; }
    public bool Next { get; }
    public bool Previous { get; }

    public CursorPager(List<T> data, Direction direction, bool hasExtra)
    {
        Data = data;
        if (direction == Direction.Next)
        {
            Previous = false;
            Next = hasExtra;
        }
        else
        {
            Next = false;
            Previous = hasExtra;
        }
    }

    public bool HasNext() => Next;
    public bool HasPrevious() => Previous;

    public PageCursor<DateTimeOffset>? GetStartCursor()
    {
        return Data.Count == 0 ? null : Data[0].GetCursor();
    }

    public PageCursor<DateTimeOffset>? GetEndCursor()
    {
        return Data.Count == 0 ? null : Data[^1].GetCursor();
    }
}

public enum Direction
{
    Prev,
    Next
}
