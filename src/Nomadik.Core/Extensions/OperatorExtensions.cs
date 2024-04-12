using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nomadik.Core.Abstractions;

namespace Nomadik.Core.Extensions;

/// <summary>
/// Extends <see cref="Operator"/>
/// </summary>
public static class OperatorExtensions 
{
    /// <summary>
    /// Converts a <see cref="Operator"/> into its respective
    /// <see cref="Expression"/> function that combines a left and right side
    /// with a logical operator.
    /// Should not be called directly, use 
    /// <see cref="SearchQuery.Compile{TIn, TOut}(Expression{Func{TIn, TOut}})"/>
    /// and its produced <see cref="CompiledSearchQuery{TIn, TOut}"/> instead.
    /// </summary>
    public static Func<Expression, Expression, Expression>? ToExpression(
        this Operator op
    )
    {
        return op switch
        {
            Operator.EQ => Expression.Equal,
            Operator.NE => Expression.NotEqual,
            Operator.GT => Expression.GreaterThan,
            Operator.LT => Expression.LessThan,
            Operator.GTE => Expression.GreaterThanOrEqual,
            Operator.LTE => Expression.LessThanOrEqual,
            _ => null
        };
    }

    private static MethodCallExpression Contains(
        Expression a, Expression b, Type valueType
    )
    {
        if (a is MethodCallExpression m)
        {
            if (
                m.Method.Name == nameof(Enumerable.ToList) ||
                m.Method.Name == nameof(Enumerable.ToArray)
            )
            {
                a = m.Arguments.First();
            }
        }

        // There's multiple Contains functions, we want the simpler one
        var contains = typeof(Enumerable)
            .GetMethods(
                BindingFlags.NonPublic | 
                BindingFlags.Public | 
                BindingFlags.Static
            ).Single(m => 
                m.Name == nameof(Enumerable.Contains) &&
                m.GetParameters().Length == 2
            ).MakeGenericMethod(valueType);

        return Expression.Call(contains, a, b);
    }

    private static Expression StringCompare(
        Expression a, 
        Expression b, 
        Func<Expression, Expression, Expression> wrapper
    )
    {
        var compare = typeof(string)
            .GetMethods(
                BindingFlags.NonPublic | 
                BindingFlags.Public | 
                BindingFlags.Static
            ).Single(m => 
                m.Name == nameof(string.Compare) &&
                m.GetParameters().Length == 2
            );

        var left = Expression.Call(compare, a, b);
        var right = Expression.Constant(0);

        return wrapper(left, right);
    }
}
