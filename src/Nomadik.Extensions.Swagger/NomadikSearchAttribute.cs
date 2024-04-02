namespace Nomadik.Extensions.Swagger;


[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class NomadikSearchAttribute(Type dto) : Attribute 
{
   public Type DTO { get; } = dto;
}
