using GdNet.Common;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace GdNetRavenDbExt
{
    public static class RavenQueryableExtensions
    {
        /// <summary>
        /// Get a paginated list of items from the queryable source.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="query">The source query</param>
        /// <param name="pageNumber">Page number, default to 0</param>
        /// <param name="pageSize">Page size, default to 10</param>
        /// <returns>A paginated list from whole source</returns>
        public static async Task<PaginatedResult<T>> GetPaginatedList<T>(this IRavenQueryable<T> query, int pageNumber, int pageSize, QueryCustomization? options = default)
        {
            var pageNumberValue = (pageNumber < 0) ? 0 : pageNumber;
            var pageSizeValue = (pageSize < 1) ? 10 : pageSize;

            var items = await query.Statistics(out var stats)
                .Customize(CustomizeQuery())
                .Skip(pageNumberValue * pageSizeValue)
                .Take(pageSizeValue)
                .ToListAsync();

            return PaginatedResult.Create(items, stats.TotalResults, pageNumberValue, pageSizeValue);

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
}
