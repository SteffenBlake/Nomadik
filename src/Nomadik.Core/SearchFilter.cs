using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Nomadik.Core.Converters;

namespace Nomadik.Core;

/// <summary>
/// Generic Discriminated Search Filter for recursive filtering
/// </summary>
[JsonConverter(typeof(SearchFilterConverter))]
public abstract class SearchFilter
{
    /// <summary>
    /// Serializes a filter into a logical expression
    /// Should not be called directly, use 
    /// <see cref="SearchQuery.Compile{TIn, TOut}(Expression{Func{TIn, TOut}})"/>
    /// and its produced <see cref="CompiledSearchQuery{TIn, TOut}"/> instead.
    /// </summary>
    public abstract Expression Compile(
        IReadOnlyDictionary<string, Expression> ctx
    );
}
