using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Nomadik.Core.Abstractions;

public interface INomadikOperationHandler
{
    public bool TryHandle<TIn, TOut>(
        INomadik<TIn, TOut> context, 
        Operator op,
        Expression expression,
        object value,
        [NotNullWhen(returnValue:true)]
        out Expression? result
    );
}
