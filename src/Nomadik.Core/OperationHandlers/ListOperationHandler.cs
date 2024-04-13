using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Nomadik.Core.Abstractions;

namespace Nomadik.Core.OperationHandlers;

public class ListOperationHandler : INomadikOperationHandler
{
    public bool TryHandle<TIn, TOut>(
        INomadik<TIn, TOut> context, 
        Operator op, 
        Expression expression, 
        object value, 
        [NotNullWhen(true)] 
        out Expression? result
    )
    {
        Func<Expression, Expression, Expression>? wrapper = op switch {
            Operator.CO => (a, b) => ListContains(a, b, value.GetType()),
            Operator.All => ListAll(context, value),
            Operator.Any => ListAny(context, value),
            _ => null
        };

        var constant = Expression.Constant(value);
        result = wrapper?.Invoke(expression, constant);

        return result != null;
    }

    private static MethodCallExpression ListContains(
        Expression a, Expression b, Type valueType
    )
    {
        var dehydrated = Dehydrate(a);

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

        return Expression.Call(contains, dehydrated, b);
    }

    private static Func<Expression, Expression, Expression>? ListAll<TIn, TOut>(
        INomadik<TIn, TOut> context,
        object value
    )
    {
        if (value is not JsonDocument doc)
        {
            return null;
        }

        throw new NotImplementedException();
    }

    private static Func<Expression, Expression, Expression>? ListAny<TIn, TOut>(
        INomadik<TIn, TOut> context,
        object value
    )
    {
        if (value is not JsonDocument doc)
        {
            return null;
        }

        throw new NotImplementedException();
    }

    // TODO : This probably could be redone as an ExpressionVisitor
    // Especially since the Hydration method could have other methods appended
    // after it too! So we need to work our way up the tree and prune everything
    // after and including the highest order hydration method
    // Example: 
    // bars = foos.Where(...)>>Prune here<<.ToArray().Where(...).ToList()
    // Right now the above example would only get that last trailing ToList

    /// <summary>
    /// It's common for JOIN'd subqueries that are projected
    /// onto a list/array/enumerable to require hydration with ORMs
    /// like EF Core and etc. As a result, the ORM requires a call to
    /// a method like ToList or ToArray.
    /// We cannot append our expression logic after that, as its post hydration.
    /// So we instead extract off that last method, and insert our logic
    /// right before it.
    /// </summary>
    private static Expression Dehydrate(Expression e)
    {
        if (e is not MethodCallExpression m)
        {
            return e;
        }

        if (
            m.Method.Name != nameof(Enumerable.ToList) &&
            m.Method.Name != nameof(Enumerable.ToArray)
        )
        {
            return e;
        }

        return m.Arguments.First();
    }
}
