using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Nomadik.Core.Converters;
using Nomadik.Core.Extensions;

namespace Nomadik.Core;

/// <summary>
/// Logical Leaf terminated operation of a <see cref="SearchFilter"/>
/// that handles a WHERE filter of the data, based on a 
/// <see cref="Key"/>, <see cref="Operator"/>, and compared <see cref="Value"/>
/// </summary>
public class SearchOperation
{
    /// <summary>
    /// Field to filter on
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// Operation to use for the filter function
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<Operator>))]
    public required Operator Operator { get; init; }

    /// <summary>
    /// Value to compare against
    /// </summary>
    [JsonConverter(typeof(ObjectToInferredTypesConverter))]
    public required object Value { get; init; }

    /// <summary>
    /// Serializes this Operation into a logical expression
    /// Should not be called directly, use 
    /// <see cref="SearchQuery.Compile{TIn, TOut}(Expression{Func{TIn, TOut}})"/>
    /// and its produced <see cref="CompiledSearchQuery{TIn, TOut}"/> instead.
    /// </summary>
    public Expression Compile(IReadOnlyDictionary<string, Expression> ctx)
    {
        var valueConst = Expression.Constant(Value);
        return Operator.ToExpression(Value.GetType())
        (
            ctx[Key.ToLower()], 
            valueConst
        );
    }
}
