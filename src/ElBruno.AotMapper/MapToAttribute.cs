namespace ElBruno.AotMapper;

/// <summary>
/// Marks a source type to generate mapping extension methods TO a destination type.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
public sealed class MapToAttribute : Attribute
{
    public MapToAttribute(Type destinationType)
    {
        DestinationType = destinationType;
    }

    public Type DestinationType { get; }
}
