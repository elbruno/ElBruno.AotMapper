using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ElBruno.AotMapper.Generator.Models;

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

        // Generate mapping code
        context.RegisterSourceOutput(allMappings, static (spc, mappings) =>
        {
            foreach (var mappingInfo in mappings)
            {
                if (mappingInfo is null)
                    continue;

                GenerateMapping(spc, mappingInfo.Value);
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
            var propertyRenames = new Dictionary<string, string>();
            var mapPropertyAttributes = typeSymbol.GetAttributes()
                .Where(a => a.AttributeClass?.ToDisplayString() == MapPropertyAttributeName);

            foreach (var mapPropAttr in mapPropertyAttributes)
            {
                if (mapPropAttr.ConstructorArguments.Length >= 2 &&
                    mapPropAttr.ConstructorArguments[0].Value is string sourceProp &&
                    mapPropAttr.ConstructorArguments[1].Value is string destProp)
                {
                    propertyRenames[destProp] = sourceProp;
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

            return new MappingInfo
            {
                SourceType = sourceType,
                DestinationType = destinationType,
                IsStrictMode = isStrictMode,
                PropertyRenames = propertyRenames,
                Compilation = context.SemanticModel.Compilation
            };
        }

        return null;
    }

    private static void GenerateMapping(SourceProductionContext context, MappingInfo mappingInfo)
    {
        var diagnostics = new List<Diagnostic>();
        
        var model = MappingParser.ParseMapping(
            mappingInfo.DestinationType,
            mappingInfo.SourceType,
            mappingInfo.IsStrictMode,
            mappingInfo.PropertyRenames,
            mappingInfo.Compilation,
            diagnostics.Add);

        // Report diagnostics
        foreach (var diagnostic in diagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }

        if (model is null)
            return;

        // Generate source code
        var source = MappingEmitter.EmitMappingClass(model);
        
        // Create a unique file name
        var fileName = $"{model.DestinationTypeName}MappingExtensions.g.cs";
        var hintName = $"{model.DestinationTypeFullName.Replace("global::", "").Replace(".", "_").Replace("<", "_").Replace(">", "_")}_{model.SourceTypeName}_Mapping.g.cs";
        
        context.AddSource(hintName, source);
    }

    private struct MappingInfo
    {
        public INamedTypeSymbol SourceType { get; set; }
        public INamedTypeSymbol DestinationType { get; set; }
        public bool IsStrictMode { get; set; }
        public Dictionary<string, string> PropertyRenames { get; set; }
        public Compilation Compilation { get; set; }
    }
}

