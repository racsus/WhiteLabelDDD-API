using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WhiteLabel.Domain.Extensions;
using WhiteLabel.Domain.Pagination;

namespace System.Linq.Expressions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second,
            Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }


        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        public static Expression<Func<T, bool>> Equals<T, TValue>(string propertyName, TValue constant)
        {
            var type = typeof(T);
            var parameterExpression = Expression.Parameter(type, type.Name);
            return
                Expression.Lambda<Func<T, bool>>(
                    Expression.Equal(Expression.Property(parameterExpression, propertyName),
                        Expression.Constant(constant)), parameterExpression);
        }

        public static string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            var unaryExpression = propertyLambda.Body as UnaryExpression;
            var memberExpression = unaryExpression == null
                ? propertyLambda.Body as MemberExpression
                : unaryExpression.Operand as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException(
                    "You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
            }

            return memberExpression.Member.Name;
        }

        public static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> propertyLambda)
        {
            var unaryExpression = propertyLambda.Body as UnaryExpression;
            var memberExpression = unaryExpression == null
                ? propertyLambda.Body as MemberExpression
                : unaryExpression.Operand as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException(
                    "You must pass a lambda of the form: 'e => Class.Property' or 'e => e.Property'");
            }

            return memberExpression.Member.Name;
        }

        public static string GetPropertyPath<T, TProperty>(Expression<Func<T, TProperty>> propertyLambda)
        {
            var unaryExpression = propertyLambda.Body as UnaryExpression;
            var memberExpression = unaryExpression == null
                ? propertyLambda.Body as MemberExpression
                : unaryExpression.Operand as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException(
                    "You must pass a lambda of the form: 'e => Class.Property' or 'e => e.Property'");
            }

            var memberName = GetMemberExpressionName(memberExpression);



            return memberName;
        }

        //TODO: Change to multiple return type expression
        public static MemberAccessModel GetMemberAccessData<T>(string propertyName)
        {
            Type memberType = null;

            var propertyRoute = propertyName.Split('.');
            MemberExpression memberAccess = null;
            var entityType = typeof(T);

            ParameterExpression memberAccessParam = Expression.Parameter(entityType);
            foreach (var route in propertyRoute)
            {
                var propertyInfo = entityType.GetProperties().FirstOrDefault(p => p.Name.Equals(route, StringComparison.OrdinalIgnoreCase));
                if (propertyInfo == null)
                    throw new ArgumentException($"Property '{route}' doesn't exist on type '{entityType}'");

                memberType = propertyInfo.PropertyType;

                var x = memberAccess ?? (Expression)memberAccessParam;

                var propertyType = propertyInfo.DeclaringType != propertyInfo.ReflectedType ? propertyInfo.DeclaringType : propertyInfo.ReflectedType;
                memberAccess = Expression.Property(x, propertyType, propertyInfo.Name);

                entityType = propertyInfo.PropertyType;
            }


            if (memberAccess == null)
                throw new ArgumentException($"Property '{propertyName}' doesn't exist on type '{typeof(T)}'");

            return new MemberAccessModel(memberAccess, memberAccessParam, memberType);
        }

        //TODO: Change to multiple return type expression
        public static Tuple<Expression, Type> MakeMemberSelectorExpression<T>(string propertyName)
        {
            var member = GetMemberAccessData<T>(propertyName);

            // build lambda expression: item => item.fieldName
            var keySelectorExp = Expression.Lambda(member.MemberAccess, member.MemberAccessParam);
            return new Tuple<Expression, Type>(keySelectorExp, member.MemberType);
        }

        //TODO: Change to multiple return type expression
        public static Expression<Func<T, string>> MakeLambdaSelectorExpression<T>(string propertyName)
        {
            var member = GetMemberAccessData<T>(propertyName);

            // build lambda expression: item => item.fieldName
            return Expression.Lambda<Func<T, string>>(member.MemberAccess, member.MemberAccessParam);
        }

        //TODO: Change to multiple return type expression
        public static Tuple<Expression, ParameterExpression, Type> MakePropertyExpression<T>(string propertyName)
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
                    var propertyInfo = type.GetProperties().FirstOrDefault(p => p.Name.Equals(route, StringComparison.OrdinalIgnoreCase));
                    if (propertyInfo != null)
                    {
                        propertyType = propertyInfo.PropertyType;
                        propertyAccess = Expression.Property(propertyAccess ?? memberAccessParam, route);
                    }
                    else
                    {
                        throw new ArgumentException("Property missing");
                    }
                }

            }
            return new Tuple<Expression, ParameterExpression, Type>(propertyAccess, memberAccessParam, propertyType);
        }

        private static string GetMemberExpressionName(MemberExpression expression)
        {
            var memberName = expression.Member.Name;

            if (expression.Expression.NodeType == ExpressionType.MemberAccess)
            {
                memberName = GetMemberExpressionName((MemberExpression)expression.Expression) + "." + memberName;
            }

            return memberName;
        }

        public static MethodInfo GetMethodInfo(Type type, string methodName, int parametersLength)
        {
            return type.GetMethods().First(m => m.Name == methodName && m.GetParameters().Length == parametersLength);
        }

        public static Expression StartsWithExpression(Expression left, Expression right)
        {
            var method = StringExtensions.StartsWithMethod;
            IEnumerable<Expression> parameters = new[]
            {
                right //, Expression.Constant(StringComparison.InvariantCultureIgnoreCase)
            };
            var exp = Expression.Call(left, method, parameters);

            return exp;
        }

        public static Expression EndsWithExpression(Expression left, Expression right)
        {
            var method = StringExtensions.EndsWithMethod;
            IEnumerable<Expression> parameters = new[]
            {
                right //, Expression.Constant(StringComparison.InvariantCultureIgnoreCase)
            };
            var exp = Expression.Call(left, method, parameters);

            return exp;
        }

        public static Expression ContainsWithExpression(Expression left, Expression right)
        {
            var method = StringExtensions.ContainsMethod;
            IEnumerable<Expression> parameters = new[]
            {
                right
            };
            var exp = Expression.Call(left, method, parameters);

            return exp;
        }

        public static Expression IsContainedInExpression(Expression left, Expression right, Type propertyType)
        {
            var method = EnumerableExtensions.ContainsMethod.MakeGenericMethod(propertyType);
            IEnumerable<Expression> parameters = new[]
            {
                right, left
            };
            var exp = Expression.Call(null, method, parameters);

            return exp;
        }

        public static Expression IsContainedInDateExpression(Expression left, Expression right, Type propertyType)
        {
            Expression res = null;
            var value = right.ToString().Replace("\"", "");
            if (int.TryParse(value, out int n))
            {
                if (value.Length <= 3)
                {
                    left = Expression.Property(left, "Month");
                }
                else
                {
                    left = Expression.Property(left, "Year");
                }

                res = Expression.Equal(left, Expression.Constant(Convert.ToInt32(value)));
            }
            else if (DateTime.TryParse(value, out DateTime d))
            {
                res = Expression.Equal(left, Expression.Constant(Convert.ToDateTime(value)));
            }
            return res;
        }

        public static Expression GetAssignExpressionString(FilterOperator filterOperator,
            Expression left, Expression right, Type propertyType)
        {
            switch (filterOperator)
            {
                case FilterOperator.IsEqualTo:
                    return Expression.Equal(left, right);
                case FilterOperator.IsNotEqualTo:
                    return Expression.NotEqual(left, right);
                case FilterOperator.StartsWith:
                    return ExpressionExtensions.StartsWithExpression(left, right);
                case FilterOperator.EndsWith:
                    return ExpressionExtensions.EndsWithExpression(left, right);
                case FilterOperator.Contains:
                    return ExpressionExtensions.ContainsWithExpression(left, right);
                case FilterOperator.DoesNotContain:
                    var contains = ExpressionExtensions.ContainsWithExpression(left, right);
                    return Expression.Not(contains);
                case FilterOperator.IsContainedIn:
                    return ExpressionExtensions.IsContainedInExpression(left, right, propertyType);
                case FilterOperator.IsNotContainedIn:
                    var isContainedIn = ExpressionExtensions.IsContainedInExpression(left, right, propertyType);
                    return Expression.Not(isContainedIn);
                //Comparisons can't be applied to strings
                case FilterOperator.IsGreaterThan:
                case FilterOperator.IsLessThan:
                case FilterOperator.IsGreaterThanOrEqualTo:
                case FilterOperator.IsLessThanOrEqualTo:
                    return null;
                default:
                    return null;
            }
        }

        public static Expression GetAssignExpressionNumeric(FilterOperator filterOperator,
            Expression left, Expression right, Type propertyType)
        {
            switch (filterOperator)
            {
                case FilterOperator.IsEqualTo:
                    return Expression.Equal(left, right);
                case FilterOperator.IsNotEqualTo:
                    return Expression.NotEqual(left, right);
                case FilterOperator.IsGreaterThan:
                    return Expression.GreaterThan(left, right);
                case FilterOperator.IsLessThan:
                    return Expression.LessThan(left, right);
                case FilterOperator.IsGreaterThanOrEqualTo:
                    return Expression.GreaterThanOrEqual(left, right);
                case FilterOperator.IsLessThanOrEqualTo:
                    return Expression.LessThanOrEqual(left, right);
                case FilterOperator.IsContainedIn:
                    return ExpressionExtensions.IsContainedInExpression(left, right, propertyType);
                case FilterOperator.IsNotContainedIn:
                    var isContainedIn = ExpressionExtensions.IsContainedInExpression(left, right, propertyType);
                    return Expression.Not(isContainedIn);
                //Comparisons can't be applied to numeric
                case FilterOperator.Contains:
                    return Expression.Equal(left, right);  // Exception
                case FilterOperator.DoesNotContain:
                case FilterOperator.StartsWith:
                case FilterOperator.EndsWith:
                    return null;
                default:
                    return null;
            }
        }

        public static Expression GetAssignExpressionBoolean(FilterOperator filterOperator,
            Expression left, Expression right, Type propertyType)
        {
            switch (filterOperator)
            {
                case FilterOperator.IsEqualTo:
                    return Expression.Equal(left, right);
                case FilterOperator.IsNotEqualTo:
                    return Expression.NotEqual(left, right);
                case FilterOperator.IsContainedIn:
                    return ExpressionExtensions.IsContainedInExpression(left, right, propertyType);
                case FilterOperator.IsNotContainedIn:
                    var isContainedIn = ExpressionExtensions.IsContainedInExpression(left, right, propertyType);
                    return Expression.Not(isContainedIn);
                //Comparisons can't be applied to boolean
                case FilterOperator.Contains:
                case FilterOperator.DoesNotContain:
                case FilterOperator.IsGreaterThan:
                case FilterOperator.IsLessThan:
                case FilterOperator.IsGreaterThanOrEqualTo:
                case FilterOperator.IsLessThanOrEqualTo:
                case FilterOperator.StartsWith:
                case FilterOperator.EndsWith:
                    return null;
                default:
                    return null;
            }
        }

        public static Expression GetAssignExpressionDateTime(FilterOperator filterOperator,
            Expression left, Expression right, Type propertyType)
        {
            switch (filterOperator)
            {
                case FilterOperator.IsEqualTo:
                    return Expression.Equal(left, right);
                case FilterOperator.IsNotEqualTo:
                    return Expression.NotEqual(left, right);
                case FilterOperator.IsGreaterThan:
                    return Expression.GreaterThan(left, right);
                case FilterOperator.IsLessThan:
                    return Expression.LessThan(left, right);
                case FilterOperator.IsGreaterThanOrEqualTo:
                    return Expression.GreaterThanOrEqual(left, right);
                case FilterOperator.IsLessThanOrEqualTo:
                    return Expression.LessThanOrEqual(left, right);
                case FilterOperator.IsContainedIn:
                    return ExpressionExtensions.IsContainedInExpression(left, right, propertyType);
                case FilterOperator.IsNotContainedIn:
                    var isContainedIn = ExpressionExtensions.IsContainedInExpression(left, right, propertyType);
                    return Expression.Not(isContainedIn);
                //Comparisons can't be applied to DateTime
                case FilterOperator.Contains:
                    // Filter by month and year
                    return ExpressionExtensions.IsContainedInDateExpression(left, right, propertyType);
                case FilterOperator.DoesNotContain:
                case FilterOperator.StartsWith:
                case FilterOperator.EndsWith:
                    return null;
                default:
                    return null;
            }
        }

        public static Expression GetAssignExpressionEnum(FilterOperator filterOperator,
            Expression left, ConstantExpression right, Type propertyType)
        {
            Type enumType;
            UnaryExpression convertExpression;
            UnaryExpression convertValue;

            switch (filterOperator)
            {
                case FilterOperator.IsEqualTo:
                    return Expression.Equal(left, right);
                case FilterOperator.IsNotEqualTo:
                    return Expression.NotEqual(left, right);
                case FilterOperator.IsGreaterThan:
                    enumType = Enum.GetUnderlyingType(propertyType);
                    convertExpression = Expression.Convert(left, enumType);
                    convertValue = Expression.Convert(right, enumType);
                    return Expression.GreaterThan(convertExpression, convertValue);
                case FilterOperator.IsLessThan:
                    enumType = Enum.GetUnderlyingType(propertyType);
                    convertExpression = Expression.Convert(left, enumType);
                    convertValue = Expression.Convert(right, enumType);
                    return Expression.LessThan(convertExpression, convertValue);
                case FilterOperator.IsGreaterThanOrEqualTo:
                    enumType = Enum.GetUnderlyingType(propertyType);
                    convertExpression = Expression.Convert(left, enumType);
                    convertValue = Expression.Convert(right, enumType);
                    return Expression.GreaterThanOrEqual(convertExpression, convertValue);
                case FilterOperator.IsLessThanOrEqualTo:
                    enumType = Enum.GetUnderlyingType(propertyType);
                    convertExpression = Expression.Convert(left, enumType);
                    convertValue = Expression.Convert(right, enumType);
                    return Expression.LessThanOrEqual(convertExpression, convertValue);
                case FilterOperator.IsContainedIn:
                    return ExpressionExtensions.IsContainedInExpression(left, right, propertyType);
                case FilterOperator.IsNotContainedIn:
                    var isContainedIn = ExpressionExtensions.IsContainedInExpression(left, right, propertyType);
                    return Expression.Not(isContainedIn);
                //Comparisons can't be applied to Enums
                case FilterOperator.Contains:
                case FilterOperator.DoesNotContain:
                case FilterOperator.StartsWith:
                case FilterOperator.EndsWith:
                    return null;
                default:
                    return null;
            }
        }

    }

    public class MemberAccessModel
    {
        public MemberExpression MemberAccess { get; set; }
        public ParameterExpression MemberAccessParam { get; set; }
        public Type MemberType { get; set; }

        public MemberAccessModel(MemberExpression memberAccess, ParameterExpression memberAccessParam, Type memberType)
        {
            MemberAccess = memberAccess;
            MemberAccessParam = memberAccessParam;
            MemberType = memberType;
        }
    }
}