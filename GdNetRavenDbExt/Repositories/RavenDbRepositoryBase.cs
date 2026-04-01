using GdNet.Common;
using GdNetDDD.Entities;
using GdNetDDD.Repositories;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace GdNetRavenDbExt.Repositories;

public abstract class RavenDbRepositoryBase<TAggregate, TId>(IAsyncDocumentSession documentSession) : RepositoryBase<TAggregate, TId>
    where TAggregate : IAggregate<TId>
{
    public override async Task<long> CountAllAsync(CancellationToken cancellationToken = default)
    {
        return await documentSession.Query<TAggregate>().LongCountAsync();
    }

    public override async Task<TAggregate> CreateOrUpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        await documentSession.StoreAsync(aggregate);

        return aggregate;
    }

    public override Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        var idString = id as string;

        documentSession.Delete(idString);

        return Task.CompletedTask;
    }

    public override async Task<TAggregate> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        // We switch to use load by ids because when a record is not found, RavenDB returns null in the dictionary
        // Otherwise the LoadAsync method throws exception InvalidCastException
        var result = await GetByIdsAsync([id]);

        return result.FirstOrDefault()!;
    }

    public override async Task<IList<TAggregate>> GetByIdsAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
    {
        var idsString = ids.Cast<string>();
        var result = await documentSession.LoadAsync<TAggregate>(idsString);

        // When a record is not found, RavenDB returns null in the dictionary
        return (result == null) ? [] : result.Values.Where(x => x != null).ToList();
    }

    public override async Task<PaginatedResult<TAggregate>> GetListAsync(int? page, int? pageSize, CancellationToken cancellationToken = default)
    {
        var query = documentSession.Query<TAggregate>();

        return await query.GetPaginatedList(page.GetPageNumberOrDefault(), pageSize.GetPageSizeOrDefault());
    }
}
