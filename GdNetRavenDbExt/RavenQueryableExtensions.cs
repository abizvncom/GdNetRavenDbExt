using GdNet.Common;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace GdNetRavenDbExt;

public static class RavenQueryableExtensions
{
    /// <summary>
    /// Get a paginated list of items from the queryable source.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="query">The source query</param>
    /// <param name="pageNumber">Page number, zero based</param>
    /// <param name="pageSize">Page size, must be >= 1</param>
    /// <returns>A paginated list from whole source</returns>
    public static async Task<PaginatedResult<T>> GetPaginatedList<T>(this IRavenQueryable<T> query, int pageNumber, int pageSize, QueryCustomization? options = default)
    {
        var items = await query.Statistics(out var stats)
            .Customize(CustomizeQuery())
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return PaginatedResult.Create(items, stats.TotalResults, pageNumber, pageSize);

        Action<IDocumentQueryCustomization> CustomizeQuery()
        {
            return c =>
            {
                if (options == null)
                {
                    return;
                }
                if (options.NoTracking)
                {
                    c.NoTracking();
                }
                if (options.NoCaching)
                {
                    c.NoCaching();
                }
            };
        }
    }
}
