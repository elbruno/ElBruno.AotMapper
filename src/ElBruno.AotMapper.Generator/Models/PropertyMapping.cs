namespace ElBruno.AotMapper.Generator.Models;

internal sealed class PropertyMapping
{
    public string SourcePropertyName { get; set; } = string.Empty;
    public string DestinationPropertyName { get; set; } = string.Empty;
    public string SourcePropertyType { get; set; } = string.Empty;
    public string DestinationPropertyType { get; set; } = string.Empty;
    public bool IsIgnored { get; set; }
    public bool NeedsConversion { get; set; }
    public string? ConverterType { get; set; }
    public bool IsNullable { get; set; }
    public bool IsDestinationNullable { get; set; }
    public MappingStrategy Strategy { get; set; }
}

internal enum MappingStrategy
{
    Direct,
    NestedMapping,
    CollectionMapping,
    EnumToEnum,
    EnumToString,
    StringToEnum,
    CustomConverter
}
