using Nomadik.Core.Abstractions;

namespace Nomadik.Core;

public class NomadikConfig : INomadikConfig
{
    public IEqualityComparer<string> KeyComparer { get; set; }
        = StringComparer.InvariantCultureIgnoreCase;
}
