using Nomadik.Core.Abstractions;

namespace Nomadik.Core;

/// <inheritdoc/>
public class NomadikConfig : INomadikConfig
{
    /// <inheritdoc/>
    public IEqualityComparer<string> KeyComparer { get; set; }
        = StringComparer.InvariantCultureIgnoreCase;
}
