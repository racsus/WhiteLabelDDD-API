using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq
{
    public static class QueryableExtensions
    {
        public static readonly MethodInfo OrderByMethod = ExpressionExtensions.GetMethodInfo(typeof(Queryable), "OrderBy", 2);
        public static readonly MethodInfo OrderByDescendingMethod = ExpressionExtensions.GetMethodInfo(typeof(Queryable), "OrderByDescending", 2);
        public static readonly MethodInfo ThenByMethod = ExpressionExtensions.GetMethodInfo(typeof(Queryable), "ThenBy", 2);
        public static readonly MethodInfo ThenByDescendingMethod = ExpressionExtensions.GetMethodInfo(typeof(Queryable), "ThenByDescending", 2);

        public static readonly MethodInfo selectT = (from x in typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                     where x.Name == nameof(Queryable.Select) && x.IsGenericMethod
                                                     let gens = x.GetGenericArguments()
                                                     where gens.Length == 2
                                                     let pars = x.GetParameters()
                                                     where pars.Length == 2 &&
                                                         pars[0].ParameterType == typeof(IQueryable<>).MakeGenericType(gens[0]) &&
                                                         pars[1].ParameterType == typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(gens))
                                                     select x).Single();

        public static IOrderedQueryable<T> OrderByPropertyName<T>(this IQueryable<T> dataCollection, MethodInfo orderOperator, string propertyName)
        {
            var keySelectorValue = ExpressionExtensions.MakeMemberSelectorExpression<T>(propertyName);
            // build concrete MethodInfo from the generic one
            orderOperator = orderOperator.MakeGenericMethod(typeof(T), keySelectorValue.Item2);
            // invoke method on dataCollection
            return orderOperator.Invoke(null, new object[] { dataCollection, keySelectorValue.Item1 }) as IOrderedQueryable<T>;
        }

        public static IQueryable<T> SelectByPropertyName<T>(this IQueryable<T> dataCollection, string propertyName)
        {
            var entityType = typeof(T);
            var prop = entityType.GetProperty(propertyName);
            var source = Expression.Parameter(entityType, "ss");

            var func = typeof(Func<,>);
            var genericFunc = func.MakeGenericType(typeof(T), prop.PropertyType);

            var exp = Expression.Lambda(Expression.Property(source, prop), source);

            MethodInfo select = selectT.MakeGenericMethod(entityType, prop.PropertyType);
            return (IQueryable<T>)select.Invoke(null, new object[] { dataCollection, exp });
        }

    }
}
