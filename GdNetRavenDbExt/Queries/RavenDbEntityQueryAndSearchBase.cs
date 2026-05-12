using GdNet.Common;
using GdNetDDD.Models;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace GdNetRavenDbExt.Queries;

using GdNetDDD.Domain.Queries;

public abstract class RavenDbEntityQueryAndSearchBase<TEntity>(IAsyncDocumentSession documentSession, string indexName)
    : IGenericEntityQuery<TEntity>, IGenericEntitySearch<TEntity>
        where TEntity : IEntity
{
    protected IAsyncDocumentSession DocumentSession => documentSession;

    public Task<PaginatedResult<TEntity>> FilterAsync(string value, IEnumerable<string> filterableFields, int? page, int? pageSize,
        params SortOption<TEntity>[] sortOptions)
    {
        var query = GetSearchableQuery();

        foreach (var fieldName in filterableFields)
        {
            query = ApplyFilter(query, fieldName, value, QueryOperator.Guess);
        }

        return query.ApplySortOptions(sortOptions)
                    .GetPaginatedList(page.GetPageNumberOrDefault(), pageSize.GetPageSizeOrDefault());
    }

    public Task<PaginatedResult<TEntity>> FilterAsync(IEnumerable<QueryOption> filterOptions, int? page, int? pageSize,
        params SortOption<TEntity>[] sortOptions)
    {
        var query = GetSearchableQuery();

        foreach (var option in filterOptions)
        {
            query = ApplyFilter(query, option.Field, option.Value, option.Operator);
        }

        return query.ApplySortOptions(sortOptions)
                    .GetPaginatedList(page.GetPageNumberOrDefault(), pageSize.GetPageSizeOrDefault());
    }

    public Task<PaginatedResult<TEntity>> SearchAsync(string ftsTerms, IEnumerable<string> searchableFields, int? page, int? pageSize,
        params SortOption<TEntity>[] sortOptions)
    {
        var query = GetSearchableQuery();

        foreach (var fieldName in searchableFields)
        {
            query = ApplySearch(query, fieldName, ftsTerms, boost: 1);
        }

        return query.ApplySortOptions(sortOptions)
                    .GetPaginatedList(page.GetPageNumberOrDefault(), pageSize.GetPageSizeOrDefault());
    }

    public Task<PaginatedResult<TEntity>> SearchAsync(IEnumerable<SearchOption> searchOptions, int? page, int? pageSize,
        params SortOption<TEntity>[] sortOptions)
    {
        var query = GetSearchableQuery();

        foreach (var option in searchOptions)
        {
            query = ApplySearch(query, option.Field, option.FtsTerms, boost: 1);
        }

        return query.ApplySortOptions(sortOptions)
                    .GetPaginatedList(page.GetPageNumberOrDefault(), pageSize.GetPageSizeOrDefault());
    }

    protected virtual IRavenQueryable<TEntity> GetSearchableQuery()
    {
        return documentSession.Query<TEntity>(indexName);
    }

    protected void ValidateStringFilterOperator(QueryOperator filterOperator)
    {
        IEnumerable<QueryOperator> supportedOperators = [QueryOperator.Equals, QueryOperator.NotEquals, QueryOperator.Guess];

        if (!supportedOperators.Contains(filterOperator))
        {
            throw new ArgumentException($"Operator {filterOperator} is not supported for string fields. It must be Equals or NotEquals.");
        }
    }

    protected abstract IRavenQueryable<TEntity> ApplyFilter(IRavenQueryable<TEntity> query, string field, object value, QueryOperator filterOperator);

    protected abstract IRavenQueryable<TEntity> ApplySearch(IRavenQueryable<TEntity> query, string field, string terms, int boost = 1);
}
