using Microsoft.CodeAnalysis;

namespace ElBruno.AotMapper.Generator;

internal static class DiagnosticDescriptors
{
    private const string Category = "AotMapper";
    private const string HelpLinkUriBase = "https://github.com/elbruno/ElBruno.AotMapper/blob/main/docs/diagnostics.md";

    public static readonly DiagnosticDescriptor MissingSourceProperty = new(
        id: "AOTMAP001",
        title: "Missing source property",
        messageFormat: "Property '{0}' on '{1}' has no matching source property on '{2}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkUriBase}#aotmap001");

    public static readonly DiagnosticDescriptor TypeMismatch = new(
        id: "AOTMAP002",
        title: "Type mismatch",
        messageFormat: "Property '{0}' type '{1}' does not match source type '{2}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkUriBase}#aotmap002");

    public static readonly DiagnosticDescriptor NullabilityMismatch = new(
        id: "AOTMAP003",
        title: "Nullability mismatch",
        messageFormat: "Source property '{0}' is nullable but destination is non-nullable",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkUriBase}#aotmap003");

    public static readonly DiagnosticDescriptor DuplicateMapping = new(
        id: "AOTMAP004",
        title: "Duplicate mapping",
        messageFormat: "Type '{0}' has conflicting [MapFrom] or [MapTo] definitions",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkUriBase}#aotmap004");

    public static readonly DiagnosticDescriptor InvalidConverter = new(
        id: "AOTMAP005",
        title: "Invalid converter",
        messageFormat: "Converter type '{0}' does not implement IMapConverter<{1}, {2}>",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkUriBase}#aotmap005");

    public static readonly DiagnosticDescriptor InaccessibleMember = new(
        id: "AOTMAP006",
        title: "Inaccessible member",
        messageFormat: "Source property '{0}' on type '{1}' is not accessible (private or internal without InternalsVisibleTo)",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkUriBase}#aotmap006");

    public static readonly DiagnosticDescriptor ProjectionNotEfCompatible = new(
        id: "AOTMAP007",
        title: "Projection not EF-compatible",
        messageFormat: "Mapping from '{0}' to '{1}' cannot generate ProjectTo method: {2}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkUriBase}#aotmap007");
}
