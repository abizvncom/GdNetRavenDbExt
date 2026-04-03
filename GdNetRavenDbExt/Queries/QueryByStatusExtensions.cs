using GdNetDDD.Entities;
using GdNetDDD.Errors;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace GdNetRavenDbExt.Queries;

using GdNetDDD.Queries;

public static class QueryByStatusExtensions
{
    public static IRavenQueryable<TEntity> FilterByStatus<TEntity, TStatus>(this IRavenQueryable<TEntity> query, object value, QueryOperator filterOperator)
        where TEntity : ITrackableEntity<TStatus>
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
            var statuses = GetStatusList();
            return query.Where(t => t.Status.In(statuses));
        }

        if (filterOperator == QueryOperator.NotEquals)
        {
            var statuses = GetStatusList();
            return query.Where(t => !t.Status.In(statuses));
        }

        throw new DomainException($"Operator {filterOperator} is not supported.", default);

        IEnumerable<TStatus> GetStatusList()
        {
            return [(TStatus)value];
        }
    }
}
