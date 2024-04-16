using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Nomadik.Core.Abstractions;

/// <summary>
/// Extensible middleware service for custom operation handling 
/// on Search Operations
/// </summary>
public interface INomadikOperationHandler
{
    /// <summary>
    /// Middleware method for handling of a passed in Search Operation
    /// </summary>
    /// <param name="context">
    /// The complete composition of known mappings and expression tree 
    /// for this search operation
    /// </param>
    /// <param name="op">
    /// The current Operator to serialize against
    /// </param>
    /// <param name="expression">
    /// The existing "left" side of the operation that is to built on top of
    /// </param>
    /// <param name="value">
    /// The passed in value to be compared against
    /// </param>
    /// <param name="result">
    /// The output composed Expression to be serialized using all the other params
    /// </param>
    /// <returns>
    /// Whether or not the operation was handled. 
    ///
    /// Returning false will signal for Nomadik to move forward 
    /// onto the next OperationHandler middleware.
    ///
    /// Returning true will signal to Nomadik serialization was handled by
    /// this middleware and to return with the produced expression.
    /// </returns>
    public bool TryHandle<TIn, TOut>(
        INomadik<TIn, TOut> context, 
        Operator op,
        Expression expression,
        object? value,
        [NotNullWhen(returnValue:true)]
        out Expression? result
    );
}
