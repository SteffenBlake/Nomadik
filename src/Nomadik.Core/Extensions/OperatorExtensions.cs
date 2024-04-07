using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

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
    public static Func<Expression, Expression, Expression> ToExpression(
        this Operator op, Type valueType
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

            Operator.LI => Like,
            Operator.CO => (a,b) => Contains(a, b, valueType),
            _ => throw new NotImplementedException()
        };
    }

    private static MethodCallExpression Like(
        Expression a, Expression b
    )
    {
        var toString = Expression.Call(a, nameof(ToString), Type.EmptyTypes);

        var dbFunctions = Expression.Constant(EF.Functions);

        // There's two Like functions, we want the simpler one
        var like = typeof(DbFunctionsExtensions)
            .GetMethods(
                BindingFlags.NonPublic | 
                BindingFlags.Public | 
                BindingFlags.Static
            ).Single(m => 
                m.Name == nameof(DbFunctionsExtensions.Like) &&
                m.GetParameters().Length == 3
            );

        return Expression.Call(like, dbFunctions, toString, b);
    }

    private static MethodCallExpression Contains(
        Expression a, Expression b, Type valueType
    )
    {
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
}
