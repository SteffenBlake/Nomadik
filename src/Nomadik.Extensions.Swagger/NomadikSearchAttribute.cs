namespace Nomadik.Extensions.Swagger;
/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class NomadikSearchAttribute(Type dto) : Attribute 
{
    /// <summary>
    ///
    /// </summary>
    public Type DTO { get; } = dto;
}
