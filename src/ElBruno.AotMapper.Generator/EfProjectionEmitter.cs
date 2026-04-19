using System.Linq;
using System.Text;
using ElBruno.AotMapper.Generator.Models;

namespace ElBruno.AotMapper.Generator;

internal static class EfProjectionEmitter
{
    public static (string? Code, string? Reason) TryEmitProjectionMethod(MappingModel model)
    {
        // Check if this mapping is EF-compatible
        var incompatibilityReason = GetEfIncompatibilityReason(model);
        if (incompatibilityReason != null)
        {
            return (null, incompatibilityReason);
        }

        var sb = new StringBuilder();
        var methodName = $"ProjectTo{model.DestinationTypeName}";
        
        sb.AppendLine();
        sb.AppendLine("        /// <summary>");
        sb.AppendLine($"        /// Projects IQueryable&lt;{model.SourceTypeName}&gt; to IQueryable&lt;{model.DestinationTypeName}&gt;.");
        sb.AppendLine("        /// This method generates an EF Core-compatible expression tree for server-side projection.");
        sb.AppendLine("        /// </summary>");
        sb.AppendLine($"        public static System.Linq.IQueryable<{model.DestinationTypeFullName}> {methodName}(this System.Linq.IQueryable<{model.SourceTypeFullName}> source)");
        sb.AppendLine("        {");
        sb.AppendLine("            if (source is null)");
        sb.AppendLine("                throw new System.ArgumentNullException(nameof(source));");
        sb.AppendLine();
        sb.Append("            return source.Select(x => new ");
        sb.Append(model.DestinationTypeFullName);

        if (model.UseConstructorMapping)
        {
            EmitConstructorProjection(sb, model);
        }
        else
        {
            EmitObjectInitializerProjection(sb, model);
        }

        sb.AppendLine("        }");

        return (sb.ToString(), null);
    }

    private static void EmitConstructorProjection(StringBuilder sb, MappingModel model)
    {
        sb.Append("(");
        
        var parameters = model.PropertyMappings
            .Where(m => !m.IsIgnored && model.ConstructorParameterNames.Contains(m.DestinationPropertyName))
            .OrderBy(m => model.ConstructorParameterNames.IndexOf(m.DestinationPropertyName))
            .ToList();

        for (int i = 0; i < parameters.Count; i++)
        {
            if (i > 0)
                sb.Append(", ");
            
            sb.Append(EmitPropertyProjection(parameters[i]));
        }

        sb.AppendLine("));");
    }

    private static void EmitObjectInitializerProjection(StringBuilder sb, MappingModel model)
    {
        sb.AppendLine();
        sb.AppendLine("            {");

        var properties = model.PropertyMappings.Where(m => !m.IsIgnored).ToList();
        
        for (int i = 0; i < properties.Count; i++)
        {
            var mapping = properties[i];
            sb.Append($"                {mapping.DestinationPropertyName} = ");
            sb.Append(EmitPropertyProjection(mapping));
            
            if (i < properties.Count - 1)
                sb.Append(",");
            
            sb.AppendLine();
        }

        sb.AppendLine("            });");
    }

    private static string EmitPropertyProjection(PropertyMapping mapping)
    {
        var sourceAccess = $"x.{mapping.SourcePropertyName}";

        switch (mapping.Strategy)
        {
            case MappingStrategy.Direct:
                return sourceAccess;

            case MappingStrategy.NestedMapping:
                // Nested mapping via navigation property
                var destTypeName = ExtractSimpleTypeName(mapping.DestinationPropertyType);
                if (mapping.IsNullable)
                {
                    return $"{sourceAccess} == null ? null : new {mapping.DestinationPropertyType} {{ {GetNestedProjectionProperties(mapping)} }}";
                }
                else
                {
                    return $"new {mapping.DestinationPropertyType} {{ {GetNestedProjectionProperties(mapping)} }}";
                }

            case MappingStrategy.EnumToString:
                if (mapping.IsNullable)
                    return $"{sourceAccess} == null ? null : {sourceAccess}.ToString()";
                return $"{sourceAccess}.ToString()";

            case MappingStrategy.EnumToEnum:
                return $"({mapping.DestinationPropertyType}){sourceAccess}";

            default:
                return sourceAccess;
        }
    }

    private static string GetNestedProjectionProperties(PropertyMapping mapping)
    {
        // For nested mappings, we need to inline the property mappings
        // This is a simplified version - in practice, we'd need to look up the nested mapping model
        // For now, assume simple property-to-property mapping
        return "/* nested properties */";
    }

    private static string? GetEfIncompatibilityReason(MappingModel model)
    {
        foreach (var mapping in model.PropertyMappings.Where(m => !m.IsIgnored))
        {
            switch (mapping.Strategy)
            {
                case MappingStrategy.CustomConverter:
                    return "contains custom converter (not EF-translatable)";

                case MappingStrategy.CollectionMapping:
                    return "contains collection mapping (not supported in ProjectTo)";

                case MappingStrategy.StringToEnum:
                    return "contains String-to-Enum conversion (use Enum.Parse in post-processing)";
            }
        }

        return null;
    }

    private static string ExtractSimpleTypeName(string fullTypeName)
    {
        var name = fullTypeName.Replace("global::", "");
        var lastDot = name.LastIndexOf('.');
        if (lastDot >= 0)
        {
            name = name.Substring(lastDot + 1);
        }
        name = name.TrimEnd('?');
        var genericStart = name.IndexOf('<');
        if (genericStart >= 0)
        {
            name = name.Substring(0, genericStart);
        }
        return name;
    }
}
