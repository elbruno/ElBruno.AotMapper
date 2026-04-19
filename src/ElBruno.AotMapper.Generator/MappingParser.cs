using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using ElBruno.AotMapper.Generator.Models;

namespace ElBruno.AotMapper.Generator;

internal static class MappingParser
{
    public static MappingModel? ParseMapping(
        INamedTypeSymbol destinationType,
        INamedTypeSymbol sourceType,
        bool isStrictMode,
        Dictionary<string, string> propertyRenames,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic)
    {
        var model = new MappingModel
        {
            SourceTypeName = sourceType.Name,
            SourceTypeNamespace = sourceType.ContainingNamespace?.ToDisplayString() ?? string.Empty,
            SourceTypeFullName = sourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            DestinationTypeName = destinationType.Name,
            DestinationTypeNamespace = destinationType.ContainingNamespace?.ToDisplayString() ?? string.Empty,
            DestinationTypeFullName = destinationType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            IsDestinationRecord = destinationType.IsRecord,
            IsDestinationStruct = destinationType.TypeKind == TypeKind.Struct,
            IsStrictMode = isStrictMode
        };

        // Get source properties
        var sourceProperties = GetProperties(sourceType).ToDictionary(p => p.Name, p => p);

        // Check if destination is a record with primary constructor
        var primaryConstructor = destinationType.Constructors
            .FirstOrDefault(c => c.Parameters.Length > 0 && !c.IsStatic);

        if (model.IsDestinationRecord && primaryConstructor != null)
        {
            model.UseConstructorMapping = true;
            foreach (var param in primaryConstructor.Parameters)
            {
                model.ConstructorParameterNames.Add(param.Name);
            }
        }

        // Get destination properties (or constructor parameters for records)
        IEnumerable<(string Name, ITypeSymbol Type, bool IsNullable, ISymbol Symbol)> destinationMembers;

        if (model.UseConstructorMapping && primaryConstructor != null)
        {
            destinationMembers = primaryConstructor.Parameters.Select(p => 
                (p.Name, p.Type, p.NullableAnnotation == NullableAnnotation.Annotated, (ISymbol)p));
        }
        else
        {
            destinationMembers = GetProperties(destinationType)
                .Where(p => !p.IsReadOnly || model.IsDestinationRecord)
                .Select(p => (p.Name, p.Type, p.NullableAnnotation == NullableAnnotation.Annotated, (ISymbol)p));
        }

        foreach (var (destName, destType, isDestNullable, destSymbol) in destinationMembers)
        {
            // Check for MapIgnore attribute
            if (HasMapIgnoreAttribute(destSymbol))
            {
                continue; // Skip this property/parameter
            }
            
            // Check for property rename
            var sourceName = destName;
            if (propertyRenames.TryGetValue(destName, out var renamed))
            {
                sourceName = renamed;
            }

            if (!sourceProperties.TryGetValue(sourceName, out var sourceProp))
            {
                if (isStrictMode)
                {
                    reportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.MissingSourceProperty,
                        Location.None,
                        destName,
                        destinationType.Name,
                        sourceType.Name));
                }
                continue;
            }

            // Check if source property is accessible
            if (!IsAccessible(sourceProp, destinationType))
            {
                reportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.InaccessibleMember,
                    Location.None,
                    sourceName,
                    sourceType.Name));
                continue; // Skip this property
            }

            // Check for MapConverter attribute
            var converterType = GetMapConverterType(destSymbol);
            MappingStrategy strategy;
            string? converterTypeName = null;

            if (converterType != null)
            {
                // Validate converter implements IMapConverter<TSource, TDest>
                if (ValidateConverter(converterType, sourceProp.Type, destType, compilation))
                {
                    strategy = MappingStrategy.CustomConverter;
                    converterTypeName = converterType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                }
                else
                {
                    // Report diagnostic for invalid converter
                    reportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.InvalidConverter,
                        Location.None,
                        converterType.ToDisplayString(),
                        sourceProp.Type.ToDisplayString(),
                        destType.ToDisplayString()));
                    continue; // Skip this property
                }
            }
            else
            {
                strategy = DetermineStrategy(sourceProp.Type, destType, compilation);
            }

            var mapping = new PropertyMapping
            {
                SourcePropertyName = sourceName,
                DestinationPropertyName = destName,
                SourcePropertyType = sourceProp.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                DestinationPropertyType = destType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                IsNullable = sourceProp.NullableAnnotation == NullableAnnotation.Annotated,
                IsDestinationNullable = isDestNullable,
                Strategy = strategy,
                ConverterType = converterTypeName
            };

            // Check nullability mismatch
            if (mapping.IsNullable && !mapping.IsDestinationNullable)
            {
                reportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.NullabilityMismatch,
                    Location.None,
                    destName));
            }

            model.PropertyMappings.Add(mapping);
        }

        return model;
    }

    private static MappingStrategy DetermineStrategy(ITypeSymbol sourceType, ITypeSymbol destType, Compilation compilation)
    {
        var sourceDisplay = sourceType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var destDisplay = destType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        // Unwrap nullable types
        var sourceUnwrapped = UnwrapNullable(sourceType);
        var destUnwrapped = UnwrapNullable(destType);

        // Direct assignment if types match
        if (SymbolEqualityComparer.Default.Equals(sourceUnwrapped, destUnwrapped))
        {
            return MappingStrategy.Direct;
        }

        // Check for collection types
        if (IsCollectionType(sourceType, out var sourceElementType) && 
            IsCollectionType(destType, out var destElementType))
        {
            return MappingStrategy.CollectionMapping;
        }

        // Check for dictionary types
        if (IsDictionaryType(sourceType, out _, out _) &&
            IsDictionaryType(destType, out _, out _))
        {
            return MappingStrategy.DictionaryMapping;
        }

        // Check for enum conversions
        if (sourceUnwrapped.TypeKind == TypeKind.Enum && destUnwrapped.TypeKind == TypeKind.Enum)
        {
            return MappingStrategy.EnumToEnum;
        }

        if (sourceUnwrapped.TypeKind == TypeKind.Enum && destUnwrapped.SpecialType == SpecialType.System_String)
        {
            return MappingStrategy.EnumToString;
        }

        if (sourceUnwrapped.SpecialType == SpecialType.System_String && destUnwrapped.TypeKind == TypeKind.Enum)
        {
            return MappingStrategy.StringToEnum;
        }

        // Enum to int conversion
        if (sourceUnwrapped.TypeKind == TypeKind.Enum && IsIntegralType(destUnwrapped))
        {
            return MappingStrategy.EnumToInt;
        }

        // Int to enum conversion
        if (IsIntegralType(sourceUnwrapped) && destUnwrapped.TypeKind == TypeKind.Enum)
        {
            return MappingStrategy.IntToEnum;
        }

        // Check if destination type has mapping
        // This would require checking for MapFrom attribute on the type
        // For now, assume nested mapping if both are reference types
        if (!sourceUnwrapped.IsValueType && !destUnwrapped.IsValueType &&
            sourceUnwrapped.SpecialType == SpecialType.None && destUnwrapped.SpecialType == SpecialType.None)
        {
            return MappingStrategy.NestedMapping;
        }

        return MappingStrategy.Direct;
    }

    private static ITypeSymbol UnwrapNullable(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } namedType)
        {
            return namedType.TypeArguments[0];
        }
        return type;
    }

    private static bool IsCollectionType(ITypeSymbol type, out ITypeSymbol? elementType)
    {
        elementType = null;

        // Check for array
        if (type is IArrayTypeSymbol arrayType)
        {
            elementType = arrayType.ElementType;
            return true;
        }

        // Check for IEnumerable<T>, List<T>, ICollection<T>, etc.
        if (type is INamedTypeSymbol namedType)
        {
            // Check if it's a generic collection
            if (namedType.TypeArguments.Length == 1)
            {
                var typeName = namedType.OriginalDefinition.ToDisplayString();
                if (typeName.StartsWith("System.Collections.Generic.IEnumerable<") ||
                    typeName.StartsWith("System.Collections.Generic.List<") ||
                    typeName.StartsWith("System.Collections.Generic.ICollection<") ||
                    typeName.StartsWith("System.Collections.Generic.IReadOnlyCollection<") ||
                    typeName.StartsWith("System.Collections.Generic.IReadOnlyList<"))
                {
                    elementType = namedType.TypeArguments[0];
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsDictionaryType(ITypeSymbol type, out ITypeSymbol? keyType, out ITypeSymbol? valueType)
    {
        keyType = null;
        valueType = null;

        if (type is INamedTypeSymbol namedType && namedType.TypeArguments.Length == 2)
        {
            var typeName = namedType.OriginalDefinition.ToDisplayString();
            if (typeName.StartsWith("System.Collections.Generic.Dictionary<") ||
                typeName.StartsWith("System.Collections.Generic.IDictionary<") ||
                typeName.StartsWith("System.Collections.Generic.IReadOnlyDictionary<"))
            {
                keyType = namedType.TypeArguments[0];
                valueType = namedType.TypeArguments[1];
                return true;
            }
        }

        return false;
    }

    private static bool IsIntegralType(ITypeSymbol type)
    {
        return type.SpecialType == SpecialType.System_Int32 ||
               type.SpecialType == SpecialType.System_Int64 ||
               type.SpecialType == SpecialType.System_Int16 ||
               type.SpecialType == SpecialType.System_Byte ||
               type.SpecialType == SpecialType.System_SByte ||
               type.SpecialType == SpecialType.System_UInt32 ||
               type.SpecialType == SpecialType.System_UInt64 ||
               type.SpecialType == SpecialType.System_UInt16;
    }

    private static bool HasMapIgnoreAttribute(ISymbol symbol)
    {
        const string mapIgnoreAttributeName = "ElBruno.AotMapper.MapIgnoreAttribute";
        return symbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == mapIgnoreAttributeName);
    }

    private static bool IsAccessible(IPropertySymbol property, INamedTypeSymbol destinationType)
    {
        // Check if property is public
        if (property.DeclaredAccessibility == Accessibility.Public)
            return true;

        // Check if property is internal and destination type is in the same assembly
        if (property.DeclaredAccessibility == Accessibility.Internal)
        {
            var sourceAssembly = property.ContainingAssembly;
            var destAssembly = destinationType.ContainingAssembly;

            // Same assembly - accessible
            if (SymbolEqualityComparer.Default.Equals(sourceAssembly, destAssembly))
                return true;

            // Check for InternalsVisibleTo
            foreach (var attr in sourceAssembly.GetAttributes())
            {
                if (attr.AttributeClass?.Name == "InternalsVisibleToAttribute" &&
                    attr.ConstructorArguments.Length > 0 &&
                    attr.ConstructorArguments[0].Value is string assemblyName)
                {
                    // Simple name comparison (could be more sophisticated)
                    if (destAssembly.Name == assemblyName.Split(',')[0].Trim())
                        return true;
                }
            }
        }

        // Private, protected, or other - not accessible
        return false;
    }

    private static INamedTypeSymbol? GetMapConverterType(ISymbol symbol)
    {
        const string mapConverterAttributeName = "ElBruno.AotMapper.MapConverterAttribute";
        var converterAttr = symbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == mapConverterAttributeName);

        if (converterAttr == null || converterAttr.ConstructorArguments.Length == 0)
            return null;

        return converterAttr.ConstructorArguments[0].Value as INamedTypeSymbol;
    }

    private static bool ValidateConverter(INamedTypeSymbol converterType, ITypeSymbol sourceType, ITypeSymbol destType, Compilation compilation)
    {
        // Check if converter implements IMapConverter<TSource, TDest>
        var mapConverterInterface = compilation.GetTypeByMetadataName("ElBruno.AotMapper.IMapConverter`2");
        if (mapConverterInterface == null)
            return false;

        // Construct the expected interface: IMapConverter<sourceType, destType>
        var expectedInterface = mapConverterInterface.Construct(sourceType, destType);

        // Check if converter implements the expected interface
        foreach (var @interface in converterType.AllInterfaces)
        {
            if (SymbolEqualityComparer.Default.Equals(@interface, expectedInterface))
                return true;
        }

        return false;
    }

    private static IEnumerable<IPropertySymbol> GetProperties(ITypeSymbol type)
    {
        return type.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => !p.IsStatic && !p.IsIndexer);
    }
}
