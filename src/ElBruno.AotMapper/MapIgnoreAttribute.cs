namespace ElBruno.AotMapper;

/// <summary>
/// Marks a destination property or constructor parameter to be ignored during mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class MapIgnoreAttribute : Attribute
{
}
