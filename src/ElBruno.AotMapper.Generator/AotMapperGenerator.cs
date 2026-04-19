using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ElBruno.AotMapper.Generator.Models;
using System;

namespace ElBruno.AotMapper.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class AotMapperGenerator : IIncrementalGenerator
{
    private const string MapFromAttributeName = "ElBruno.AotMapper.MapFromAttribute";
    private const string MapToAttributeName = "ElBruno.AotMapper.MapToAttribute";
    private const string MapPropertyAttributeName = "ElBruno.AotMapper.MapPropertyAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Find types with [MapFrom] attribute
        var mapFromCandidates = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                MapFromAttributeName,
                predicate: static (node, _) => node is TypeDeclarationSyntax,
                transform: static (ctx, _) => GetMappingInfo(ctx, isMapFrom: true))
            .Where(static m => m is not null);

        // Find types with [MapTo] attribute
        var mapToCandidates = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                MapToAttributeName,
                predicate: static (node, _) => node is TypeDeclarationSyntax,
                transform: static (ctx, _) => GetMappingInfo(ctx, isMapFrom: false))
            .Where(static m => m is not null);

        // Combine both sources
        var allMappings = mapFromCandidates.Collect()
            .Combine(mapToCandidates.Collect())
            .Select(static (pair, _) => pair.Left.Concat(pair.Right).ToImmutableArray());

        // Combine with compilation
        var mappingsWithCompilation = allMappings.Combine(context.CompilationProvider);

        // Generate mapping code
        context.RegisterSourceOutput(mappingsWithCompilation, static (spc, data) =>
        {
            var (mappings, compilation) = data;
            
            // Check for duplicate mappings (same source + dest combination)
            var seen = new HashSet<(string, string)>();
            var duplicates = new HashSet<(string, string)>();
            
            foreach (var mappingInfo in mappings)
            {
                if (mappingInfo is null)
                    continue;

                var key = (mappingInfo.Value.SourceTypeFullName, mappingInfo.Value.DestinationTypeFullName);
                if (!seen.Add(key))
                {
                    duplicates.Add(key);
                }
            }

            // Report duplicate mapping diagnostics
            foreach (var (sourceType, destType) in duplicates)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.DuplicateMapping,
                    Location.None,
                    destType);
                spc.ReportDiagnostic(diagnostic);
            }

            // Generate mappings (skip duplicates)
            var processed = new HashSet<(string, string)>();
            foreach (var mappingInfo in mappings)
            {
                if (mappingInfo is null)
                    continue;

                var key = (mappingInfo.Value.SourceTypeFullName, mappingInfo.Value.DestinationTypeFullName);
                
                // Skip if duplicate
                if (duplicates.Contains(key))
                    continue;
                    
                // Skip if already processed
                if (!processed.Add(key))
                    continue;

                GenerateMapping(spc, mappingInfo.Value, compilation);
            }
        });
    }

    private static MappingInfo? GetMappingInfo(GeneratorAttributeSyntaxContext context, bool isMapFrom)
    {
        var typeSymbol = context.TargetSymbol as INamedTypeSymbol;
        if (typeSymbol is null)
            return null;

        var attributes = typeSymbol.GetAttributes();
        
        foreach (var attribute in attributes)
        {
            var attributeName = attribute.AttributeClass?.ToDisplayString();
            if (attributeName != (isMapFrom ? MapFromAttributeName : MapToAttributeName))
                continue;

            if (attribute.ConstructorArguments.Length == 0)
                continue;

            var otherType = attribute.ConstructorArguments[0].Value as INamedTypeSymbol;
            if (otherType is null)
                continue;

            // Get Strict property for MapFrom
            bool isStrictMode = false;
            if (isMapFrom)
            {
                var strictArg = attribute.NamedArguments.FirstOrDefault(a => a.Key == "Strict");
                if (strictArg.Value.Value is bool strictValue)
                {
                    isStrictMode = strictValue;
                }
            }

            // Get property renames from [MapProperty] attributes
            var propertyRenamesBuilder = ImmutableArray.CreateBuilder<PropertyRename>();
            var mapPropertyAttributes = typeSymbol.GetAttributes()
                .Where(a => a.AttributeClass?.ToDisplayString() == MapPropertyAttributeName);

            foreach (var mapPropAttr in mapPropertyAttributes)
            {
                if (mapPropAttr.ConstructorArguments.Length >= 2 &&
                    mapPropAttr.ConstructorArguments[0].Value is string sourceProp &&
                    mapPropAttr.ConstructorArguments[1].Value is string destProp)
                {
                    propertyRenamesBuilder.Add(new PropertyRename(destProp, sourceProp));
                }
            }

            INamedTypeSymbol sourceType, destinationType;
            
            if (isMapFrom)
            {
                sourceType = otherType;
                destinationType = typeSymbol;
            }
            else
            {
                sourceType = typeSymbol;
                destinationType = otherType;
            }

            return new MappingInfo(
                sourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                destinationType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                isStrictMode,
                new EquatableArray<PropertyRename>(propertyRenamesBuilder.ToImmutable())
            );
        }

        return null;
    }

    private static void GenerateMapping(SourceProductionContext context, MappingInfo mappingInfo, Compilation compilation)
    {
        var diagnostics = new List<Diagnostic>();

        // Resolve type symbols from full names
        var sourceType = compilation.GetTypeByMetadataName(mappingInfo.SourceTypeFullName.Replace("global::", ""));
        var destinationType = compilation.GetTypeByMetadataName(mappingInfo.DestinationTypeFullName.Replace("global::", ""));

        if (sourceType is null || destinationType is null)
            return;

        // Convert EquatableArray back to Dictionary for parser
        var propertyRenames = new Dictionary<string, string>();
        foreach (var rename in mappingInfo.PropertyRenames)
        {
            propertyRenames[rename.DestinationProperty] = rename.SourceProperty;
        }
        
        var model = MappingParser.ParseMapping(
            destinationType,
            sourceType,
            mappingInfo.IsStrictMode,
            propertyRenames,
            compilation,
            diagnostics.Add);

        // Report diagnostics
        foreach (var diagnostic in diagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }

        if (model is null)
            return;

        // Check EF compatibility and report diagnostic if not compatible
        var (_, incompatibilityReason) = EfProjectionEmitter.TryEmitProjectionMethod(model);
        if (incompatibilityReason != null)
        {
            var location = destinationType.Locations.FirstOrDefault() ?? Location.None;
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.ProjectionNotEfCompatible,
                location,
                sourceType.Name,
                destinationType.Name,
                incompatibilityReason);
            context.ReportDiagnostic(diagnostic);
        }

        // Generate source code
        var source = MappingEmitter.EmitMappingClass(model);
        
        // Create a unique file name
        var fileName = $"{model.DestinationTypeName}MappingExtensions.g.cs";
        var hintName = $"{model.DestinationTypeFullName.Replace("global::", "").Replace(".", "_").Replace("<", "_").Replace(">", "_")}_{model.SourceTypeName}_Mapping.g.cs";
        
        context.AddSource(hintName, source);
    }

    private readonly struct MappingInfo : IEquatable<MappingInfo>
    {
        public MappingInfo(string sourceTypeFullName, string destinationTypeFullName, bool isStrictMode, EquatableArray<PropertyRename> propertyRenames)
        {
            SourceTypeFullName = sourceTypeFullName;
            DestinationTypeFullName = destinationTypeFullName;
            IsStrictMode = isStrictMode;
            PropertyRenames = propertyRenames;
        }

        public string SourceTypeFullName { get; }
        public string DestinationTypeFullName { get; }
        public bool IsStrictMode { get; }
        public EquatableArray<PropertyRename> PropertyRenames { get; }

        public bool Equals(MappingInfo other)
        {
            return SourceTypeFullName == other.SourceTypeFullName &&
                   DestinationTypeFullName == other.DestinationTypeFullName &&
                   IsStrictMode == other.IsStrictMode &&
                   PropertyRenames.Equals(other.PropertyRenames);
        }

        public override bool Equals(object? obj) => obj is MappingInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = SourceTypeFullName?.GetHashCode() ?? 0;
                hash = (hash * 397) ^ (DestinationTypeFullName?.GetHashCode() ?? 0);
                hash = (hash * 397) ^ IsStrictMode.GetHashCode();
                hash = (hash * 397) ^ PropertyRenames.GetHashCode();
                return hash;
            }
        }
    }

    private readonly struct PropertyRename : IEquatable<PropertyRename>
    {
        public PropertyRename(string destinationProperty, string sourceProperty)
        {
            DestinationProperty = destinationProperty;
            SourceProperty = sourceProperty;
        }

        public string DestinationProperty { get; }
        public string SourceProperty { get; }

        public bool Equals(PropertyRename other)
        {
            return DestinationProperty == other.DestinationProperty &&
                   SourceProperty == other.SourceProperty;
        }

        public override bool Equals(object? obj) => obj is PropertyRename other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((DestinationProperty?.GetHashCode() ?? 0) * 397) ^ (SourceProperty?.GetHashCode() ?? 0);
            }
        }
    }
}

