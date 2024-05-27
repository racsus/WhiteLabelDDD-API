using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WhiteLabel.Domain.Extensions;
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Domain.Pagination
{
    public interface ISpecificationBuilder
    {
        ISpecification<T> Create<T>(IEnumerable<FilterDescriptor> filters);
    }

    public class SpecificationBuilder : ISpecificationBuilder
    {
        private static readonly MethodInfo CreateExpressionMethodInfo =
            typeof(SpecificationBuilder).GetMethod(
                nameof(CreateExpression),
                BindingFlags.Instance | BindingFlags.NonPublic
            );

        public const char AllCharOperator = '&';
        public const char AnyCharOperator = '|';

        private readonly char[] CompositeCharOperators = { AllCharOperator, AnyCharOperator };

        private readonly IDictionary<char, MethodInfo> CompositeCharMethods = new Dictionary<
            char,
            MethodInfo
        >()
        {
            { AllCharOperator, EnumerableExtensions.AllMethod },
            { AnyCharOperator, EnumerableExtensions.AnyMethod }
        };

        //private readonly ILogger<SpecificationBuilder> logger;

        //public SpecificationBuilder(ILogger<SpecificationBuilder> logger)
        public SpecificationBuilder()
        {
            //this.logger = logger;
        }

        public ISpecification<T> Create<T>(IEnumerable<FilterDescriptor> filters)
        {
            var spec = SpecificationBase<T>.True;
            if (filters == null)
            {
                return spec;
            }

            foreach (var filter in filters)
            {
                try
                {
                    var fSpec = this.Create<T>(filter);
                    if (fSpec != null)
                    {
                        spec = spec.And(fSpec);
                    }
                }
                catch
                {
                    //this.logger.LogInformation(
                    //    ex,
                    //    $"There was an error generating the filter for {typeof(T)} - {filter.Member} - {filter.Operator} - {filter.Value}");
                }
            }

            return spec;
        }

        public SpecificationBase<T> Create<T>(FilterDescriptor filter)
        {
            var expression = this.CreateExpression<T>(filter);

            if (expression == null)
            {
                return null;
            }

            var spec = new SpecificationBase<T>(expression);
            return spec;
        }

        private Expression<Func<T, bool>> CreateExpression<T>(FilterDescriptor filter)
        {
            //TODO: use c# 7 named value tuples or create an object with more explicit names
            Expression<Func<T, bool>> expression;
            var index = filter.Member.IndexOfAny(this.CompositeCharOperators);
            if (index > -1)
            {
                expression = this.CreateMemberExpressionComposite<T>(filter, index);
            }
            else
            {
                expression = this.CreateMemberExpressionSimple<T>(filter);
            }

            return expression;
        }

        private Expression<Func<T, bool>> CreateMemberExpressionComposite<T>(
            FilterDescriptor filter,
            int index
        )
        {
            var collectionPath = filter.Member.Substring(0, index);
            var separator = filter.Member[index];
            var childPropertyPath = filter.Member.Substring(
                index + 1,
                (filter.Member.Length - (index + 1))
            );
            var childFilter = new FilterDescriptor
            {
                Member = childPropertyPath,
                Operator = filter.Operator,
                Value = filter.Value
            };

            var collectionInfo = ExpressionExtensions.MakePropertyExpression<T>(collectionPath);
            var collectionPropertyTye = collectionInfo.Item3;
            var collectionChildPropertyType = collectionPropertyTye.GenericTypeArguments[0];

            var createMethod = CreateExpressionMethodInfo.MakeGenericMethod(
                collectionChildPropertyType
            );
            var childExpression =
                createMethod.Invoke(this, new object[] { childFilter }) as Expression;

            if (childExpression == null)
                return null;

            var method = this.CompositeCharMethods[separator];
            var concreteMethod = method.MakeGenericMethod(collectionChildPropertyType);

            IEnumerable<Expression> parameters = new[] { collectionInfo.Item1, childExpression };
            var exp = Expression.Call(null, concreteMethod, parameters);
            var expression = Expression.Lambda<Func<T, bool>>(exp, collectionInfo.Item2);

            return expression;
        }

        private Expression<Func<T, bool>> CreateMemberExpressionSimple<T>(FilterDescriptor filter)
        {
            var left = ExpressionExtensions.MakePropertyExpression<T>(filter.Member);
            //TODO: change type
            var right = this.GetExpressionValue(filter, left.Item3);

            //FilterOperator Switch
            var assign = this.GetAssignExpression(left.Item3, filter.Operator, left.Item1, right);
            if (assign == null)
            {
                return null;
            }
            return Expression.Lambda<Func<T, bool>>(assign, left.Item2);
        }

        private ConstantExpression GetExpressionValue(FilterDescriptor filter, Type properType)
        {
            switch (filter.Operator)
            {
                case FilterOperator.IsLessThan:
                case FilterOperator.IsLessThanOrEqualTo:
                case FilterOperator.IsEqualTo:
                case FilterOperator.IsNotEqualTo:
                case FilterOperator.IsGreaterThan:
                case FilterOperator.IsGreaterThanOrEqualTo:
                case FilterOperator.StartsWith:
                case FilterOperator.EndsWith:
                case FilterOperator.Contains:
                case FilterOperator.DoesNotContain:
                    var effectiveValue = filter.Value.ConvertTo(properType);
                    return Expression.Constant(effectiveValue, properType);
                case FilterOperator.IsContainedIn:
                case FilterOperator.IsNotContainedIn:
                    var array = filter.Value.Split(
                        new[] { "," },
                        StringSplitOptions.RemoveEmptyEntries
                    );
                    var arrayValues = array.Select(x => x.ConvertTo(properType));
                    var castMethod = EnumerableExtensions.CastMethod;
                    var genericCastMethod = castMethod.MakeGenericMethod(properType);
                    var obj = genericCastMethod.Invoke(null, new object[] { arrayValues });
                    return Expression.Constant(obj);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Expression GetAssignExpression(
            Type propertyType,
            FilterOperator filterOperator,
            Expression left,
            ConstantExpression right
        )
        {
            var effectiveType = propertyType.GetEffectiveType();

            if (effectiveType == typeof(string))
            {
                return this.GetAssignExpressionString(filterOperator, left, right, propertyType);
            }

            if (effectiveType.IsNumericType())
            {
                return this.GetAssignExpressionNumeric(filterOperator, left, right, propertyType);
            }

            if (effectiveType == typeof(bool))
            {
                return this.GetAssignExpressionBoolean(filterOperator, left, right, propertyType);
            }

            if (effectiveType == typeof(DateTime))
            {
                return this.GetAssignExpressionDateTime(filterOperator, left, right, propertyType);
            }

            if (effectiveType.IsEnum())
            {
                return this.GetAssignExpressionEnum(filterOperator, left, right, propertyType);
            }

            return null;
        }

        private Expression StartsWithExpression(Expression left, Expression right)
        {
            var method = StringExtensions.StartsWithMethod;
            IEnumerable<Expression> parameters = new[]
            {
                right //, Expression.Constant(StringComparison.InvariantCultureIgnoreCase)
            };
            var exp = Expression.Call(left, method, parameters);

            return exp;
        }

        private Expression EndsWithExpression(Expression left, Expression right)
        {
            var method = StringExtensions.EndsWithMethod;
            IEnumerable<Expression> parameters = new[]
            {
                right //, Expression.Constant(StringComparison.InvariantCultureIgnoreCase)
            };
            var exp = Expression.Call(left, method, parameters);

            return exp;
        }

        private Expression ContainsWithExpression(Expression left, Expression right)
        {
            var method = StringExtensions.ContainsMethod;
            IEnumerable<Expression> parameters = new[] { right };
            var exp = Expression.Call(left, method, parameters);

            return exp;
        }

        private Expression IsContainedInExpression(
            Expression left,
            Expression right,
            Type propertyType
        )
        {
            //var r = new[] {"1", "2", "3"};
            //var l = "1";
            //Enumerable.Contains<string>(r, l);
            // r.Contains(l);

            var method = EnumerableExtensions.ContainsMethod.MakeGenericMethod(propertyType);
            IEnumerable<Expression> parameters = new[] { right, left };
            var exp = Expression.Call(null, method, parameters);

            return exp;
        }

        //TODO: Refactor to it custom classes

        #region - String expression

        private Expression GetAssignExpressionString(
            FilterOperator filterOperator,
            Expression left,
            Expression right,
            Type propertyType
        )
        {
            switch (filterOperator)
            {
                case FilterOperator.IsEqualTo:
                    return Expression.Equal(left, right);
                case FilterOperator.IsNotEqualTo:
                    return Expression.NotEqual(left, right);
                case FilterOperator.StartsWith:
                    return this.StartsWithExpression(left, right);
                case FilterOperator.EndsWith:
                    return this.EndsWithExpression(left, right);
                case FilterOperator.Contains:
                    return this.ContainsWithExpression(left, right);
                case FilterOperator.DoesNotContain:
                    var contains = this.ContainsWithExpression(left, right);
                    return Expression.Not(contains);
                case FilterOperator.IsContainedIn:
                    return this.IsContainedInExpression(left, right, propertyType);
                case FilterOperator.IsNotContainedIn:
                    var isContainedIn = this.IsContainedInExpression(left, right, propertyType);
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

        #endregion

        #region - Numeric expression

        private Expression GetAssignExpressionNumeric(
            FilterOperator filterOperator,
            Expression left,
            Expression right,
            Type propertyType
        )
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
                    return this.IsContainedInExpression(left, right, propertyType);
                case FilterOperator.IsNotContainedIn:
                    var isContainedIn = this.IsContainedInExpression(left, right, propertyType);
                    return Expression.Not(isContainedIn);
                //Comparisons can't be applied to numeric
                case FilterOperator.Contains:
                case FilterOperator.DoesNotContain:
                case FilterOperator.StartsWith:
                case FilterOperator.EndsWith:
                    return null;
                default:
                    return null;
            }
        }

        #endregion

        #region - Boolean expression

        private Expression GetAssignExpressionBoolean(
            FilterOperator filterOperator,
            Expression left,
            Expression right,
            Type propertyType
        )
        {
            switch (filterOperator)
            {
                case FilterOperator.IsEqualTo:
                    return Expression.Equal(left, right);
                case FilterOperator.IsNotEqualTo:
                    return Expression.NotEqual(left, right);
                case FilterOperator.IsContainedIn:
                    return this.IsContainedInExpression(left, right, propertyType);
                case FilterOperator.IsNotContainedIn:
                    var isContainedIn = this.IsContainedInExpression(left, right, propertyType);
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

        #endregion

        #region - DateTime expression

        private Expression GetAssignExpressionDateTime(
            FilterOperator filterOperator,
            Expression left,
            Expression right,
            Type propertyType
        )
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
                    return this.IsContainedInExpression(left, right, propertyType);
                case FilterOperator.IsNotContainedIn:
                    var isContainedIn = this.IsContainedInExpression(left, right, propertyType);
                    return Expression.Not(isContainedIn);
                //Comparisons can't be applied to DateTime
                case FilterOperator.Contains:
                case FilterOperator.DoesNotContain:
                case FilterOperator.StartsWith:
                case FilterOperator.EndsWith:
                    return null;
                default:
                    return null;
            }
        }

        #endregion

        #region - Enum expression

        private Expression GetAssignExpressionEnum(
            FilterOperator filterOperator,
            Expression left,
            ConstantExpression right,
            Type propertyType
        )
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
                    return this.IsContainedInExpression(left, right, propertyType);
                case FilterOperator.IsNotContainedIn:
                    var isContainedIn = this.IsContainedInExpression(left, right, propertyType);
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

        #endregion
    }
}
