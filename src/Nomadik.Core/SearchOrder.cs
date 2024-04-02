using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Nomadik.Core;

/// <summary>
/// Serializable model for defining OrderBy
/// </summary>
public class SearchOrder
{
    [JsonConverter(typeof(JsonStringEnumConverter<OrderDir>))]
    public OrderDir Dir { get; init; } = OrderDir.Asc;

    public required string By { get; init; }

    public Expression Compile(IReadOnlyDictionary<string, Expression> ctx)
    {
       return ctx[By.ToLower()]; 
    }
}
