using System.Text.Json.Serialization;
using Nomadik.Core.Converters;

namespace Nomadik.Core;

/// <summary>
/// Serializable model for defining a complex search query function
/// </summary>
public class SearchQuery
{
    /// <summary>
    /// (Optional) Search Filter logic to filter results on
    /// unfiltered results will be returned if set to null
    /// </summary>
    [JsonConverter(typeof(SearchFilterConverter))]
    public SearchFilter? Filter { get; init; } = null;

    /// <summary>
    /// (Optional) OrderBy operations to perform on the returned results,
    /// system default order will be used if set to null
    /// </summary>
    public SearchOrder? Order { get; init; } = null;

    /// <summary>
    /// (Optional) Pagination for results. All results will be returned
    /// if set to null
    /// </summary>
    public SearchPagination? Page { get; init; } = null;
}
