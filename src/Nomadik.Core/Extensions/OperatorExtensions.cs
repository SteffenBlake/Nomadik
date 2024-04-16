using System.Linq.Expressions;
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
    /// <see cref="NomadikExtensions.Compile{TIn, TOut}(INomadik{TIn, TOut}, SearchQuery)"/>
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
        };
    }
}
