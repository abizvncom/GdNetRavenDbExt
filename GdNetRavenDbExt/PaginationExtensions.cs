namespace GdNetRavenDbExt;

public static class PaginationExtensions
{
    public static int GetPageNumberOrDefault(this int? pageNumber)
    {
        if (pageNumber.HasValue && pageNumber.Value >= 0)
        {
            return pageNumber.Value;
        }

        return 0;
    }

    public static int GetPageSizeOrDefault(this int? pageSize)
    {
        if (pageSize.HasValue && pageSize.Value > 0)
        {
            return pageSize.Value;
        }

        return 10;
    }
}
