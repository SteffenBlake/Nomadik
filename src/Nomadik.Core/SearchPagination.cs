using System.Text.Json.Serialization;

namespace Nomadik.Core;

/// <summary>
/// Serializable model for defining Pagination params
/// </summary>
public class SearchPagination
{
    /// <summary>
    /// Page Size to paginate on
    /// </summary>
    public int Size { get; init; } = 20;

    /// <summary>
    /// Page Number to Paginate to
    /// </summary>
    public int Num { get; init; } = 1;

    /// <summary>
    /// Utility property that calculates the necessary count to Skip to
    /// get to the first item of the Page space
    /// </summary>
    [JsonIgnore]
    public int Skip => (Num-1) * Size;

    /// <summary>
    /// Utility property that calculates the necessary item count to Take
    /// after the requisite <see cref="Skip"/>
    /// </summary>
    [JsonIgnore]
    public int Take => Size;
}
