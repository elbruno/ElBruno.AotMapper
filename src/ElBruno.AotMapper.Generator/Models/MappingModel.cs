using System.Collections.Generic;

namespace ElBruno.AotMapper.Generator.Models;

internal sealed class MappingModel
{
    public string SourceTypeName { get; set; } = string.Empty;
    public string SourceTypeNamespace { get; set; } = string.Empty;
    public string SourceTypeFullName { get; set; } = string.Empty;
    
    public string DestinationTypeName { get; set; } = string.Empty;
    public string DestinationTypeNamespace { get; set; } = string.Empty;
    public string DestinationTypeFullName { get; set; } = string.Empty;
    
    public bool IsDestinationRecord { get; set; }
    public bool IsDestinationStruct { get; set; }
    public bool UseConstructorMapping { get; set; }
    
    public List<PropertyMapping> PropertyMappings { get; set; } = new List<PropertyMapping>();
    public List<string> ConstructorParameterNames { get; set; } = new List<string>();
    
    public bool IsStrictMode { get; set; }
}
