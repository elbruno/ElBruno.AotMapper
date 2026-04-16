namespace ElBruno.AotMapper;

/// <summary>
/// Marks a destination type for compile-time mapping from a source type.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
public sealed class MapFromAttribute : Attribute
{
    public MapFromAttribute(Type sourceType)
    {
        SourceType = sourceType;
    }

    public Type SourceType { get; }
    public bool Strict { get; set; }
}
