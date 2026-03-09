namespace RealWorld.Application.Pagination;

public class DateTimeCursor : PageCursor<DateTimeOffset>
{
    public DateTimeCursor(DateTimeOffset data) : base(data) { }

    public override string ToString()
    {
        return Data.ToUnixTimeMilliseconds().ToString();
    }

    public static DateTimeOffset? Parse(string? cursor)
    {
        if (cursor == null)
            return null;

        return DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(cursor));
    }
}
