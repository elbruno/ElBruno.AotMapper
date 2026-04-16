using Microsoft.CodeAnalysis;

namespace ElBruno.AotMapper.Generator;

internal static class DiagnosticDescriptors
{
    private const string Category = "AotMapper";

    public static readonly DiagnosticDescriptor MissingSourceProperty = new(
        id: "AOTMAP001",
        title: "Missing source property",
        messageFormat: "Property '{0}' on '{1}' has no matching source property on '{2}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TypeMismatch = new(
        id: "AOTMAP002",
        title: "Type mismatch",
        messageFormat: "Property '{0}' type '{1}' does not match source type '{2}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NullabilityMismatch = new(
        id: "AOTMAP003",
        title: "Nullability mismatch",
        messageFormat: "Source property '{0}' is nullable but destination is non-nullable",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor DuplicateMapping = new(
        id: "AOTMAP004",
        title: "Duplicate mapping",
        messageFormat: "Type '{0}' has conflicting [MapFrom] or [MapTo] definitions",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidConverter = new(
        id: "AOTMAP005",
        title: "Invalid converter",
        messageFormat: "Converter type '{0}' does not implement IMapConverter<{1}, {2}>",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
