using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nomadik.Core.Abstractions;
using Nomadik.Core.Extensions;

namespace Nomadik.Core.OperationHandlers;

/// <inheritdoc/>
public class StringOperationHandler : INomadikOperationHandler
{
    /// <inheritdoc/>
    public bool TryHandle<TIn, TOut>(
        INomadik<TIn, TOut> context, 
        Operator op, 
        Expression expression, 
        object? value, 
        [NotNullWhen(true)] 
        out Expression? result
    )
    {
        if (value is not string s && value != null)
        {
            result = default;
            return false;
        }

        Func<Expression, Expression, Expression>? wrapper = op switch {
            Operator.LI => Like,
            Operator.CO => null,
            Operator.EQ => Expression.Equal,
            Operator.NE => Expression.NotEqual,
            Operator.All => null,
            Operator.Any => null,
            _ => StringCompare(op)
        };

        var constant = Expression.Constant(value);
        result = wrapper?.Invoke(expression, constant);

        return result != null;
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

    private static Func<Expression, Expression, Expression>? StringCompare(
        Operator op
    )
    {
        var wrapper = op.ToExpression();
        if (wrapper == null)
        {
            return null;
        }

        var compare = typeof(string)
            .GetMethods(
                BindingFlags.NonPublic | 
                BindingFlags.Public | 
                BindingFlags.Static
            ).Single(m => 
                m.Name == nameof(string.Compare) &&
                m.GetParameters().Length == 2
            );

        var right = Expression.Constant(0);

        return (a, b) => 
            wrapper.Invoke(Expression.Call(compare, a, b), right);
    }
}
