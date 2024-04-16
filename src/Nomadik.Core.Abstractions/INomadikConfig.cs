namespace Nomadik.Core.Abstractions;

/// <summary>
/// The plugin wide configuration for Nomadik
/// </summary>
public interface INomadikConfig 
{
    /// <summary>
    /// How keys are searched for and matched for on Filters and OrderBys
    /// Defaults to culture invariant.
    /// </summary>
    public IEqualityComparer<string> KeyComparer { get; }
}
