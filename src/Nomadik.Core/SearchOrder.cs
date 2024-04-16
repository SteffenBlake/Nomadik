using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Nomadik.Core.Abstractions;
using Nomadik.Core.Extensions;

namespace Nomadik.Core;

/// <summary>
/// Serializable model for defining OrderBy
/// </summary>
public class SearchOrder
{
    /// <summary>
    /// The Direction to OrderBy
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<OrderDir>))]
    public OrderDir Dir { get; init; } = OrderDir.Asc;

    /// <summary>
    /// The Field to Order By
    /// </summary>
    public required string By { get; init; }

    /// <summary>
    /// Further "Then" Order By after this one (can be repeatedly nested)
    /// </summary>
    public SearchOrder? Then { get; init; } = null;

    /// <summary>
    /// Compiles this Order Operation into a logical expression.
    /// Should not be called directly, use 
    /// <see cref="NomadikExtensions.Compile{TIn, TOut}(INomadik{TIn, TOut}, SearchQuery)"/>
    /// and its produced <see cref="CompiledSearchQuery{TIn, TOut}"/> instead.
    /// </summary>
    public Expression<Func<TIn, object>> Compile<TIn, TOut>(
        INomadik<TIn, TOut> context
    )
    {
        var orderExpression = context.Lookup[By.ToLower()];
        var conversion = Expression.Convert(orderExpression, typeof(object));
        return Expression.Lambda<Func<TIn, object>>(
            conversion, context.Mapper.Parameters 
        );
    }
}
