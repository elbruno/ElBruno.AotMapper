namespace ElBruno.AotMapper;

/// <summary>
/// Maps a source property to a destination property with a different name.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
public sealed class MapPropertyAttribute : Attribute
{
    public MapPropertyAttribute(string sourceProperty, string destinationProperty)
    {
        SourceProperty = sourceProperty;
        DestinationProperty = destinationProperty;
    }

    public string SourceProperty { get; }
    public string DestinationProperty { get; }
}
