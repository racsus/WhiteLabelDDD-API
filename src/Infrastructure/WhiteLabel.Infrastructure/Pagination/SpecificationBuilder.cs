using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Pagination;
using WhiteLabel.Infrastructure.Data.Constants;
using WhiteLabel.Infrastructure.Data.Extensions;

namespace WhiteLabel.Infrastructure.Data.Pagination
{
    public interface ISpecificationBuilder
    {
        ISpecification<T> Create<T>(IEnumerable<FilterOption> filters);
    }

    public class SpecificationBuilder : ISpecificationBuilder
    {
        private static readonly MethodInfo CreateExpressionMethodInfo =
            typeof(SpecificationBuilder).GetMethod(
                nameof(CreateExpression),
                BindingFlags.Instance | BindingFlags.NonPublic
            );

        private const char AllCharOperator = '&';
        private const char AnyCharOperator = '|';

        private readonly char[] compositeCharOperators = { AllCharOperator, AnyCharOperator };

        private readonly IDictionary<char, MethodInfo> compositeCharMethods = new Dictionary<
            char,
            MethodInfo
        >()
        {
            { AllCharOperator, EnumerableExtensions.AllMethod },
            { AnyCharOperator, EnumerableExtensions.AnyMethod },
        };

        private readonly ILogger<SpecificationBuilder> logger;

        public SpecificationBuilder(ILogger<SpecificationBuilder> logger)
        {
            this.logger = logger;
        }

        public ISpecification<T> Create<T>(IEnumerable<FilterOption> filters)
        {
            var spec = SpecificationBase<T>.True;
            var repeatedMembers = new List<string>();
            if (filters == null)
                return spec;

            // And
            var filterOptions = filters as FilterOption[] ?? filters.ToArray();
            foreach (var filter in filterOptions)
                try
                {
                    // Check if we have more than 1 filter for the same member (Or)
                    var isRepeated = filterOptions.Count(x => x.Member == filter.Member) > 1;
                    if (!isRepeated)
                    {
                        var fSpec = Create<T>(filter);
                        if (fSpec != null)
                            spec = spec.And(fSpec);
                    }
                    else
                    {
                        if (repeatedMembers.All(x => x != filter.Member))
                            repeatedMembers.Add(filter.Member);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogInformation(
                        ex,
                        $"There was an error generating the filter for {typeof(T)} - {filter.Member} - {filter.Operator} - {filter.Value}"
                    );
                }

            // Or
            if (repeatedMembers.Count > 0)
            {
                var specOr = SpecificationBase<T>.False;
                foreach (var member in repeatedMembers)
                {
                    foreach (var filter in filterOptions.Where(x => x.Member == member))
                        try
                        {
                            var fSpec = Create<T>(filter);
                            if (fSpec != null)
                                specOr = specOr.Or(fSpec);
                        }
                        catch (Exception ex)
                        {
                            logger.LogInformation(
                                ex,
                                $"There was an error generating the filter for {typeof(T)} - {filter.Member} - {filter.Operator} - {filter.Value}"
                            );
                        }

                    spec = spec.And(specOr);
                }
            }

            return spec;
        }

        public SpecificationBase<T> Create<T>(FilterOption filter)
        {
            var expression = CreateExpression<T>(filter);

            if (expression == null)
                return null;

            var spec = new SpecificationBase<T>(expression);
            return spec;
        }

        private Expression<Func<T, bool>> CreateExpression<T>(FilterOption filter)
        {
            //TODO: use c# 7 named value tuples or create an object with more explicit names
            Expression<Func<T, bool>> expression;
            var index = filter.Member.IndexOfAny(compositeCharOperators);
            if (index > -1)
                expression = CreateMemberExpressionComposite<T>(filter, index);
            else
                expression = CreateMemberExpressionSimple<T>(filter);

            return expression;
        }

        private Expression<Func<T, bool>> CreateMemberExpressionComposite<T>(
            FilterOption filter,
            int index
        )
        {
            var collectionPath = filter.Member.Substring(0, index);
            var separator = filter.Member[index];
            var childPropertyPath = filter.Member.Substring(
                index + 1,
                filter.Member.Length - (index + 1)
            );
            var childFilter = new FilterOption
            {
                Member = childPropertyPath,
                Operator = filter.Operator,
                Value = filter.Value,
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

            var method = compositeCharMethods[separator];
            var concreteMethod = method.MakeGenericMethod(collectionChildPropertyType);

            IEnumerable<Expression> parameters = new[] { collectionInfo.Item1, childExpression };
            var exp = Expression.Call(null, concreteMethod, parameters);
            var expression = Expression.Lambda<Func<T, bool>>(exp, collectionInfo.Item2);

            return expression;
        }

        private Expression<Func<T, bool>> CreateMemberExpressionSimple<T>(FilterOption filter)
        {
            var left = ExpressionExtensions.MakePropertyExpression<T>(filter.Member);
            //TODO: change type
            var right = GetExpressionValue(filter, left.Item3);

            //FilterOperator Switch
            var assign = GetAssignExpression(left.Item3, filter.Operator, left.Item1, right);
            if (assign == null)
                return null;
            return Expression.Lambda<Func<T, bool>>(assign, left.Item2);
        }

        private ConstantExpression GetExpressionValue(FilterOption filter, Type properType)
        {
            // Filter by month and year
            if (
                (properType == typeof(DateTime) || properType == typeof(DateTime?))
                && (
                    int.TryParse(filter.Value, out _)
                    || Regex
                        .Match(
                            filter.Value,
                            GenericConstants.GenericMonthYearExpression,
                            RegexOptions.IgnoreCase
                        )
                        .Success
                    || Regex
                        .Match(
                            filter.Value,
                            GenericConstants.GenericYearExpression,
                            RegexOptions.IgnoreCase
                        )
                        .Success
                    || Regex
                        .Match(
                            filter.Value,
                            GenericConstants.GenericYearMonthExpression,
                            RegexOptions.IgnoreCase
                        )
                        .Success
                )
            )
                return Expression.Constant(filter.Value.ConvertTo(typeof(string)), typeof(string));

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

        private static Expression GetAssignExpression(
            Type propertyType,
            FilterOperator filterOperator,
            Expression left,
            ConstantExpression right
        )
        {
            var effectiveType = propertyType.GetEffectiveType();

            if (effectiveType == typeof(string) || effectiveType == typeof(Guid))
                return ExpressionExtensions.GetAssignExpressionString(
                    filterOperator,
                    left,
                    right,
                    propertyType
                );

            if (effectiveType.NumericType())
                return ExpressionExtensions.GetAssignExpressionNumeric(
                    filterOperator,
                    left,
                    right,
                    propertyType
                );

            if (effectiveType == typeof(bool))
                return ExpressionExtensions.GetAssignExpressionBoolean(
                    filterOperator,
                    left,
                    right,
                    propertyType
                );

            if (effectiveType == typeof(DateTime))
                return ExpressionExtensions.GetAssignExpressionDateTime(
                    filterOperator,
                    left,
                    right,
                    propertyType
                );

            if (effectiveType.Enum())
                return ExpressionExtensions.GetAssignExpressionEnum(
                    filterOperator,
                    left,
                    right,
                    propertyType
                );

            return null;
        }
    }
}
