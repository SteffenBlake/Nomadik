namespace Nomadik.Core;

/// <summary>
/// Represents a completed and serialized Search against a 
/// Query of <typeparamref name="T"/>
/// </summary>
public class SearchQueryResult<T>(
    List<T> results, int from, int of
)
{
    /// <summary>
    /// The Serialized result DTOs of the query
    /// </summary>
    public List<T> Results { get; } = results;

    /// <summary>
    /// Represents the 1 based inclusive record index
    /// That this result's page start matches up with against
    /// the set of all searched records after filtering
    /// </summary>
    public int From { get; } = from;

    /// <summary>
    /// Represents the 1 based inclusive record index
    /// That this result's page end matches up with against
    /// the set of all searched records after filtering
    /// </summary>
    public int To { get; } = from + results.Count - 1;

    /// <summary>
    /// The total number of records that match this
    /// Search's query after filtering, ignoring paging.
    /// </summary>
    public int Of { get; } = of;
}
