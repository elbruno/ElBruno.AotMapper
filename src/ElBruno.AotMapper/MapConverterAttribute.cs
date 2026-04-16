namespace ElBruno.AotMapper;

/// <summary>
/// Specifies a custom converter for a property or parameter during mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class MapConverterAttribute : Attribute
{
    public MapConverterAttribute(Type converterType)
    {
        ConverterType = converterType;
    }

    public Type ConverterType { get; }
}
