using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Nomadik.Core.Abstractions;
using Nomadik.Core.Converters;
using Nomadik.Core.Extensions;

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
    /// <see cref="NomadikExtensions.Compile{TIn, TOut}(INomadik{TIn, TOut}, SearchQuery)"/>
    /// and its produced <see cref="CompiledSearchQuery{TIn, TOut}"/> instead.
    /// </summary>
    public abstract Expression Compile<TIn, TOut>(
        INomadik<TIn, TOut> context 
    );
}
