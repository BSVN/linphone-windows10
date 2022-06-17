using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BSN.Resa.Vns.Commons.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, List<SortParameter> sortParameters)
        {
            IOrderedQueryable<T> orderedQuery = null;

            if (sortParameters == null || !sortParameters.Any())
                return query;

            Expression<Func<T, object>> orderBykeySelector = PropertyNameToLambdaExpression<T>(sortParameters.First().Key);

            if (sortParameters.First().Direction == SortDirection.Ascending)
            {
                orderedQuery = query.OrderBy(orderBykeySelector);
            }
            else if (sortParameters.First().Direction == SortDirection.Descending)
            {
                orderedQuery = query.OrderByDescending(orderBykeySelector);
            }

            if (sortParameters.Count > 1)
            {
                foreach (SortParameter param in sortParameters.Skip(1))
                {
                    Expression<Func<T, object>> thenBykeySelector = PropertyNameToLambdaExpression<T>(param.Key);

                    if (param.Direction == SortDirection.Ascending)
                    {
                        orderedQuery = orderedQuery.ThenBy(thenBykeySelector);
                    }
                    else if (param.Direction == SortDirection.Descending)
                    {
                        orderedQuery = orderedQuery.ThenByDescending(thenBykeySelector);
                    }
                }
            }

            return orderedQuery;
        }

        private static Expression<Func<T, object>> PropertyNameToLambdaExpression<T>(string propertyName)
        {
            try
            {
                ParameterExpression parameter = Expression.Parameter(typeof(T));
                MemberExpression property = Expression.Property(parameter, propertyName);
                UnaryExpression propAsObject = Expression.Convert(property, typeof(object));

                return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("is invalid.", propertyName);
            }
        }
    }
}
