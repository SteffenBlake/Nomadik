using Nomadik.Core.Abstractions;

namespace Nomadik.Core;

/// <inheritdoc cref="INomadikConfig"/>
public class NomadikConfig : INomadikConfig
{
    /// <inheritdoc/>
    public IEqualityComparer<string> KeyComparer { get; set; }
        = StringComparer.InvariantCultureIgnoreCase;
}
