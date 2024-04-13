using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Nomadik.Core.Abstractions;
using Nomadik.Core.Converters;

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
    /// <see cref="Nomadik{Tin, Tout}.Compile(SearchQuery)"/>
    /// and its produced <see cref="CompiledSearchQuery{TIn, TOut}"/> instead.
    /// </summary>
    public Expression Compile<TIn, TOut>(INomadik<TIn, TOut> context)
    {
        var expression = context.Lookup[Key];
        foreach(var handler in context.OpHandlers)
        {
            if (handler.TryHandle(
                context, 
                Operator,
                expression, 
                Value,
                out var result
            ))
            {
                return result;
            }
        }

        throw new NotImplementedException(
            $"No injected {nameof(INomadikOperationHandler)} handled the given data: Type='{Value.GetType()}' Value={Value} Operator={Operator}"
        );
    }
}
