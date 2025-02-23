using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WhiteLabel.Domain.Extensions;

namespace System.Linq.Expressions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second
        )
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second
        )
        {
            return first.Compose(second, Expression.AndAlso);
        }

        public static Expression<T> Compose<T>(
            this Expression<T> first,
            Expression<T> second,
            Func<Expression, Expression, Expression> merge
        )
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first
                .Parameters.Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second
        )
        {
            return first.Compose(second, Expression.Or);
        }

        public static Expression<Func<T, bool>> OrElse<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second
        )
        {
            return first.Compose(second, Expression.OrElse);
        }

        public static Expression<Func<T, bool>> Equals<T, TValue>(
            string propertyName,
            TValue constant
        )
        {
            var type = typeof(T);
            var parameterExpression = Expression.Parameter(type, type.Name);
            return Expression.Lambda<Func<T, bool>>(
                Expression.Equal(
                    Expression.Property(parameterExpression, propertyName),
                    Expression.Constant(constant)
                ),
                parameterExpression
            );
        }

        public static string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            var unaryExpression = propertyLambda.Body as UnaryExpression;
            var memberExpression =
                unaryExpression == null
                    ? propertyLambda.Body as MemberExpression
                    : unaryExpression.Operand as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException(
                    "You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'"
                );
            }

            return memberExpression.Member.Name;
        }

        public static string GetPropertyName<T, TProperty>(
            Expression<Func<T, TProperty>> propertyLambda
        )
        {
            var unaryExpression = propertyLambda.Body as UnaryExpression;
            var memberExpression =
                unaryExpression == null
                    ? propertyLambda.Body as MemberExpression
                    : unaryExpression.Operand as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException(
                    "You must pass a lambda of the form: 'e => Class.Property' or 'e => e.Property'"
                );
            }

            return memberExpression.Member.Name;
        }

        public static string GetPropertyPath<T, TProperty>(
            Expression<Func<T, TProperty>> propertyLambda
        )
        {
            var unaryExpression = propertyLambda.Body as UnaryExpression;
            var memberExpression =
                unaryExpression == null
                    ? propertyLambda.Body as MemberExpression
                    : unaryExpression.Operand as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException(
                    "You must pass a lambda of the form: 'e => Class.Property' or 'e => e.Property'"
                );
            }

            var memberName = GetMemberExpressionName(memberExpression);

            return memberName;
        }

        //TODO: Change to multiple return type expression
        public static Tuple<Expression, Type> MakeMemberSelectorExpression<T>(string propertyName)
        {
            Type memberType = null;

            var propertyRoute = propertyName.Split('.');
            MemberExpression memberAccess = null;
            var entityType = typeof(T);

            ParameterExpression memberAccessParam = Expression.Parameter(entityType);
            foreach (var route in propertyRoute)
            {
                var propertyInfo = entityType
                    .GetProperties()
                    .FirstOrDefault(p => p.Name.Equals(route, StringComparison.OrdinalIgnoreCase));
                if (propertyInfo == null)
                    throw new ArgumentException(
                        $"Property '{route}' doesn't exist on type '{entityType}'"
                    );

                memberType = propertyInfo.PropertyType;

                var x = memberAccess ?? (Expression)memberAccessParam;

                var propertyType =
                    propertyInfo.DeclaringType != propertyInfo.ReflectedType
                        ? propertyInfo.DeclaringType
                        : propertyInfo.ReflectedType;
                memberAccess = Expression.Property(x, propertyType, propertyInfo.Name);

                entityType = propertyInfo.PropertyType;
            }

            if (memberAccess == null)
                throw new ArgumentException(
                    $"Property '{propertyName}' doesn't exist on type '{typeof(T)}'"
                );

            // build lambda expression: item => item.fieldName
            var keySelectorExp = Expression.Lambda(memberAccess, memberAccessParam);
            return new Tuple<Expression, Type>(keySelectorExp, memberType);
        }

        //TODO: Change to multiple return type expression
        public static Tuple<Expression, ParameterExpression, Type> MakePropertyExpression<T>(
            string propertyName
        )
        {
            var entityType = typeof(T);
            var memberAccessParam = Expression.Parameter(entityType);
            Expression propertyAccess = null;
            var propertyRoute = propertyName.Split('.');
            Type propertyType = null;
            foreach (var route in propertyRoute)
            {
                if (route.IsNullOrEmpty())
                {
                    propertyType = entityType;
                    propertyAccess = memberAccessParam;
                }
                else
                {
                    var type = (propertyType ?? entityType);
                    var propertyInfo = type.GetProperties()
                        .FirstOrDefault(p =>
                            p.Name.Equals(route, StringComparison.OrdinalIgnoreCase)
                        );
                    if (propertyInfo != null)
                    {
                        propertyType = propertyInfo.PropertyType;
                        propertyAccess = Expression.Property(
                            propertyAccess ?? memberAccessParam,
                            route
                        );
                    }
                    else
                    {
                        throw new ArgumentException("Property missing");
                    }
                }
            }
            return new Tuple<Expression, ParameterExpression, Type>(
                propertyAccess,
                memberAccessParam,
                propertyType
            );
        }

        private static string GetMemberExpressionName(MemberExpression expression)
        {
            var memberName = expression.Member.Name;

            if (expression.Expression.NodeType == ExpressionType.MemberAccess)
            {
                memberName =
                    GetMemberExpressionName((MemberExpression)expression.Expression)
                    + "."
                    + memberName;
            }

            return memberName;
        }

        public static MethodInfo GetMethodInfo(Type type, string methodName, int parametersLength)
        {
            return type.GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == parametersLength);
        }
    }
}
