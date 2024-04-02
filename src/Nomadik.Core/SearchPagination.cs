using System.Text.Json.Serialization;

namespace Nomadik.Core;

/// <summary>
/// Serializable model for defining Pagination params
/// </summary>
public class SearchPagination
{
    public int Size { get; init; } = 20;

    public int Num { get; init; } = 1;

    [JsonIgnore]
    public int Skip => (Num-1) * Size;

    [JsonIgnore]
    public int Take => Size;
}
