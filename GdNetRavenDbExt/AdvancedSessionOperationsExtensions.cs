using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace GdNetRavenDbExt
{
    public static class AdvancedSessionOperationsExtensions
    {
        /// <summary>
        /// Gets all documents from the query and returns them as a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sessionAdvanced"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<List<T>> StreamToListAsync<T>(this IAsyncAdvancedSessionOperations sessionAdvanced, IRavenQueryable<T> query, CancellationToken cancellationToken = default)
        {
            var results = new List<T>();
            var queryResult = await sessionAdvanced.StreamAsync(query, out var stats, cancellationToken);

            while (await queryResult.MoveNextAsync())
            {
                results.Add(queryResult.Current.Document);
            }

            return results;
        }

        /// <summary>
        /// Gets all documents from the query and returns them as a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sessionAdvanced"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<T> StreamToList<T>(this IAdvancedSessionOperations sessionAdvanced, IQueryable<T> query)
        {
            var results = new List<T>();
            var queryResult = sessionAdvanced.Stream(query);

            while (queryResult.MoveNext())
            {
                results.Add(queryResult.Current.Document);
            }

            return results;
        }
    }
}
