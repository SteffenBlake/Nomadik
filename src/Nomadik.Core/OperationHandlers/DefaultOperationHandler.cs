using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Nomadik.Core.Abstractions;

namespace Nomadik.Core.OperationHandlers;

/// <inheritdoc/>
public class DefaultOperationHandler : INomadikOperationHandler
{
    /// <inheritdoc/>
    public bool TryHandle<TIn, TOut>(
        INomadik<TIn, TOut> context, 
        Operator op, 
        Expression expression, 
        object value,
        [NotNullWhen(returnValue:true)]
        out Expression? result
    )
    {
        Func<Expression, Expression, Expression>? wrapper = op switch
        {
            Operator.EQ => Expression.Equal,
            Operator.NE => Expression.NotEqual,
            Operator.GT => Expression.GreaterThan,
            Operator.LT => Expression.LessThan,
            Operator.GTE => Expression.GreaterThanOrEqual,
            Operator.LTE => Expression.LessThanOrEqual,
            _ => null
        };

        var constant = Expression.Constant(value);
        result = wrapper?.Invoke(expression, constant);

        return result != null;
    }
}
