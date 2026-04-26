using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using GdNetDDD.Domain.Queries;
using GdNetDDD.Models;
using GdNetDDD.Models.Errors;

namespace GdNetRavenDbExt.Queries;

public static class Extensions
{
    public static IRavenQueryable<TEntity> ApplySortOptions<TEntity>(this IRavenQueryable<TEntity> query, params SortOption<TEntity>[] sortOptions)
    {
        IRavenQueryable<TEntity> orderedQuery = null!;

        foreach (var option in sortOptions)
        {
            if (orderedQuery == null)
            {
                orderedQuery = (option.Direction == SortDirection.Asc)
                    ? query.OrderBy(option.Property)
                    : query.OrderByDescending(option.Property);
            }
            else
            {
                orderedQuery = (option.Direction == SortDirection.Asc)
                    ? orderedQuery.ThenBy(option.Property)
                    : orderedQuery.ThenByDescending(option.Property);
            }
        }

        return orderedQuery ?? query;
    }

    public static IRavenQueryable<TEntity> FilterByStatus<TEntity, TStatus>(this IRavenQueryable<TEntity> query, object value, QueryOperator filterOperator)
        where TEntity : IHasStatus<TStatus>
        where TStatus : Enum
    {
        if (filterOperator == QueryOperator.In)
        {
            var statuses = (List<TStatus>)value;
            return query.Where(t => t.Status.In(statuses));
        }

        if (filterOperator == QueryOperator.NotIn)
        {
            var statuses = (List<TStatus>)value;
            return query.Where(t => !t.Status.In(statuses));
        }

        if (filterOperator == QueryOperator.Equals)
        {
            var statuses = GetStatusAsList();
            return query.Where(t => t.Status.In(statuses));
        }

        if (filterOperator == QueryOperator.NotEquals)
        {
            var statuses = GetStatusAsList();
            return query.Where(t => !t.Status.In(statuses));
        }

        throw new DomainException($"Operator {filterOperator} is not supported.", default!);

        IEnumerable<TStatus> GetStatusAsList()
        {
            return [(TStatus)value];
        }
    }
}
