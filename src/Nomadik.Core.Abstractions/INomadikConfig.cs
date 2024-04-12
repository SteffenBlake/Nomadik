namespace Nomadik.Core.Abstractions;

public interface INomadikConfig 
{
    public IEqualityComparer<string> KeyComparer { get; }
}
