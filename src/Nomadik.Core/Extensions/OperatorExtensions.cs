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
        Func<Expression, Expression, Expression> wrapper = op switch
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

        if (valueType != typeof(string))
        {
            return wrapper;
        }

        return op switch
        {
            Operator.GT or Operator.GTE or Operator.LT or Operator.LTE =>
                (a, b) => StringCompare(a, b, wrapper),
            _ => wrapper
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
