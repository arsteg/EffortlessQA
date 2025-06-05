using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Api.Services.Implementation
{
    // Extension for dynamic sorting
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string propertyName)
        {
            return query.OrderBy(e => EF.Property<object>(e, propertyName));
        }

        public static IQueryable<T> OrderByDescendingDynamic<T>(
            this IQueryable<T> query,
            string propertyName
        )
        {
            return query.OrderByDescending(e => EF.Property<object>(e, propertyName));
        }
    }
}
