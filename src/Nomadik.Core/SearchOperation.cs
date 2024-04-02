using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Nomadik.Core.Converters;
using Nomadik.Core.Extensions;

namespace Nomadik.Core;

/// <summary>
/// Logical Leaf terminated operation of a <see cref="SearchFilter{T}"/>
/// That handles a WHERE filter of the data, based on a Key, Operator, 
/// and compared Value
/// </summary>
public class SearchOperation
{
    public required string Key { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter<Operator>))]
    public required Operator Operator { get; init; }

    [JsonConverter(typeof(ObjectToInferredTypesConverter))]
    public required object Value { get; init; }

    internal Expression Compile(IReadOnlyDictionary<string, Expression> ctx)
    {
        var valueConst = Expression.Constant(Value);
        return Operator.ToExpression()(ctx[Key.ToLower()], valueConst);
    }
}
