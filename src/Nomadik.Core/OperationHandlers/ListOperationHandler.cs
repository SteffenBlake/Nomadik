using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Nomadik.Core.Abstractions;
using Nomadik.Core.Extensions;

namespace Nomadik.Core.OperationHandlers;

/// <inheritdoc/>
public class ListOperationHandler : INomadikOperationHandler
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
        Func<Expression, Expression, Expression>? wrapper = op switch {
            Operator.CO => ListContains(),
            Operator.All => Subquery(context, value, ListAll),
            Operator.Any => Subquery(context, value, ListAny),
            _ => null
        };

        var dehydrated = Dehydrate(expression);
        var constant = Expression.Constant(value);
        result = wrapper?.Invoke(dehydrated, constant);

        return result != null;
    }

    private static Func<Expression, Expression, Expression>? ListContains()
    {
        // There's multiple Contains functions, we want the simpler one
        var contains = typeof(Enumerable)
            .GetMethods(
                BindingFlags.Public | 
                BindingFlags.Static
            ).Single(m => 
                m.Name == nameof(Enumerable.Contains) &&
                m.GetParameters().Length == 2
            );

        return (a, b) => 
        {
            // Extract the T out of IEnumerable<T>
            var valueType = a.Type.GenericTypeArguments.Single();
            var typedContains = contains.MakeGenericMethod(valueType);
            var casted = Expression.Convert(b, valueType);

            return Expression.Call(
                typedContains, 
                a, 
                casted
            );
        };
    }

    // Chains on a subquery inside of a LINQ operation.
    // IE .Any(...) or .All(...).
    // Composed from a sub filter expression
    private static Func<Expression, Expression, Expression>? Subquery<TIn, TOut>(
        INomadik<TIn, TOut> context,
        object? value,
        Func<Type, MethodInfo> methodProvider
    )
    {
        if (value is not SearchFilter filter)
        {
            return null;
        }

        var compile = filter.GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(m => m.Name == nameof(filter.Compile));

        // We arent actually comparing against b, but
        // instead using it to compile an entire subquery, so we discard it here
        // Since we used it above to compose filter
        return (a, _) =>
        {
            // Extract the T out of IEnumerable<T>
            var valueType = a.Type.GenericTypeArguments.Single();
            var chainedMethod = methodProvider(valueType);
            var typedCompile = compile.MakeGenericMethod(valueType, valueType);

            var (subContext, parameter) = SubqueryContext(context, valueType);
            var innerExpression = 
                (Expression)typedCompile.Invoke(filter, [subContext])!;

            var lambdaType = typeof(Func<,>)
                .MakeGenericType(valueType, typeof(bool));

            var innerQuery = Expression.Lambda(
                lambdaType, innerExpression, parameter
            );

            return Expression.Call(chainedMethod, a, innerQuery!);   
        };
    }

    // Provides LINQ .All(...)
    private static MethodInfo ListAll(Type valueType)
    {
        return typeof(Enumerable)
            .GetMethods(
                BindingFlags.Public | 
                BindingFlags.Static
            ).Single(m => 
                m.Name == nameof(Enumerable.All) &&
                m.GetParameters().Length == 2
            ).MakeGenericMethod(valueType);
    }


    // Provides LINQ .Any(...)
    private static MethodInfo ListAny(Type valueType)
    {
        return typeof(Enumerable)
            .GetMethods(
                BindingFlags.Public | 
                BindingFlags.Static
            ).Single(m => 
                m.Name == nameof(Enumerable.Any) &&
                m.GetParameters().Length == 2
            ).MakeGenericMethod(valueType);
    }

    private static (object, ParameterExpression)  SubqueryContext<TIn, TOut>(
        INomadik<TIn, TOut> old, Type targetType
    )
    {
        var parameter = Expression.Parameter(targetType);

        IReadOnlyDictionary<string, Expression> lookup = 
            new Dictionary<string, Expression>{{ "value", parameter }};

        var lambdaType = typeof(Func<,>)
            .MakeGenericType(targetType, targetType);

        var lambda = Expression.Lambda(lambdaType, parameter, parameter);

        var constructors = typeof(Nomadik<,>)
            .MakeGenericType(targetType, targetType)
            .GetConstructors();

        var dictType = typeof(IReadOnlyDictionary<string, Expression>); 
        var targetConstructor = constructors
            .Single(c =>
            {
                var parameters = c.GetParameters();
                return parameters.Any(p => p.ParameterType == dictType);
            }); 

        var innerContext = targetConstructor.Invoke([
            lambda, 
            lookup, 
            old.OpHandlers
        ]);

        return (innerContext, parameter);
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
