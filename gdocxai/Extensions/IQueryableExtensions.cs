using System;
using System.Linq;
using System.Linq.Expressions;

namespace Indexai.Extensions
{
    /// <summary>
    /// Herramienta para los sorts usando los nombres de las properdades.
    /// </summary>
    public static class IQueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string
        propertyName)
        {
            return (IQueryable<T>)((IQueryable)source).OrderBy(propertyName);
        }

        public static IQueryable OrderBy(this IQueryable source, string propertyName)
        {
            var x = Expression.Parameter(source.ElementType, "x");
            var body = propertyName.Split('.').Aggregate<string, Expression>(x,
            Expression.PropertyOrField);

            var selector = Expression.Lambda
             (Expression.PropertyOrField(x, propertyName), x);

            return source.Provider.CreateQuery(
               Expression.Call(typeof(Queryable), "OrderBy", new Type[] {
             source.ElementType, selector.Body.Type },
                    source.Expression, selector
                    ));
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source,
        string propertyName)
        {
            return (IQueryable<T>)((IQueryable)source).OrderByDescending(propertyName);
        }

        public static IQueryable OrderByDescending(this IQueryable source, string
        propertyName)
        {
            var x = Expression.Parameter(source.ElementType, "x");
            var selector = Expression.Lambda(Expression.PropertyOrField(x,
            propertyName), x);
            return source.Provider.CreateQuery(
                Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] {
             source.ElementType, selector.Body.Type },
                     source.Expression, selector
                     ));
        }
    }
}
