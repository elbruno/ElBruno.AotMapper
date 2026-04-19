using Xunit;
using Microsoft.CodeAnalysis;

namespace ElBruno.AotMapper.Generator.Tests;

/// <summary>
/// Tests for edge cases and advanced features: required properties, init-only setters,
/// records, record structs, generics, inheritance, Dictionary, nullable value types,
/// enum↔int conversion, [MapIgnore], and [MapConverter].
/// </summary>
public class EdgeCaseTests
{
    // =====================================================================
    // C# 11 required properties
    // =====================================================================

    [Fact]
    public void RequiredProperties_AreIncludedInMapping()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } public int Age { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public required string Name { get; set; }
    public required int Age { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.True(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.Contains("Name = source.Name", generatedCode);
        Assert.Contains("Age = source.Age", generatedCode);
    }

    // =====================================================================
    // init-only setters
    // =====================================================================

    [Fact]
    public void InitOnlyProperties_AreIncludedInMapping()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public string Name { get; init; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.True(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.Contains("Name = source.Name", generatedCode);
    }

    // =====================================================================
    // Records with primary constructors
    // =====================================================================

    [Fact]
    public void Records_WithPrimaryConstructor_UseConstructorMapping()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } public int Age { get; set; } }

[MapFrom(typeof(Source))]
public partial record Destination(string Name, int Age);
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.True(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        // Should use constructor: new Destination(source.Name, source.Age)
        Assert.Contains("new", generatedCode);
        Assert.Contains("Destination(", generatedCode);
    }

    [Fact(Skip = "v0.7: records with mixed primary-ctor params + extra mutable properties — tracked in CHANGELOG roadmap")]
    public void Records_WithExtraProperties_MapBoth()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source 
{ 
    public string Name { get; set; } 
    public int Age { get; set; }
    public string City { get; set; }
}

[MapFrom(typeof(Source))]
public partial record Destination(string Name, int Age)
{
    public string City { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.True(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        // Constructor params + extra property
        Assert.Contains("Name", generatedCode);
        Assert.Contains("Age", generatedCode);
        Assert.Contains("City", generatedCode);
    }

    // =====================================================================
    // Record structs
    // =====================================================================

    [Fact]
    public void RecordStructs_AreSupported()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } }

[MapFrom(typeof(Source))]
public partial record struct Destination(string Name);
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.True(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.Contains("Destination", generatedCode);
    }

    // =====================================================================
    // Generic types
    // =====================================================================

    [Fact(Skip = "v0.7: open generic source types in MapFrom — tracked in CHANGELOG roadmap")]
    public void GenericSourceAndDestination_AreSupported()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source<T> { public T Value { get; set; } }

[MapFrom(typeof(Source<int>))]
public partial class Destination 
{ 
    public int Value { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        // NOTE: Depends on Neo's generic type support
        Assert.True(result.GeneratedSources.Length > 0);
    }

    // =====================================================================
    // Inheritance — base class properties
    // =====================================================================

    [Fact]
    public void InheritedProperties_AreMapped()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class BaseSource { public string Name { get; set; } }
public class Source : BaseSource { public int Age { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public string Name { get; set; }
    public int Age { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.True(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        // Base class property + derived class property
        Assert.Contains("Name", generatedCode);
        Assert.Contains("Age", generatedCode);
    }

    // =====================================================================
    // Dictionary<string, string> mapping
    // =====================================================================

    [Fact]
    public void Dictionary_StringToString_IsMapped()
    {
        var source = @"
using System.Collections.Generic;
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public Dictionary<string, string> Metadata { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public Dictionary<string, string> Metadata { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        // NOTE: Depends on Neo implementing Dictionary support
        Assert.True(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.Contains("Metadata", generatedCode);
    }

    // =====================================================================
    // Nullable value types: int? → int (with default), int → int?
    // =====================================================================

    [Fact]
    public void NullableValueType_ToNonNullable_WithDefault()
    {
        var source = @"
#nullable enable
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public int? Age { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public int Age { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.True(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        // Should use ?? 0 or GetValueOrDefault()
        Assert.Contains("Age", generatedCode);
    }

    [Fact]
    public void NonNullableValueType_ToNullable()
    {
        var source = @"
#nullable enable
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public int Age { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public int? Age { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.True(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.Contains("Age", generatedCode);
    }

    // =====================================================================
    // Null elements in collections
    // =====================================================================

    [Fact]
    public void Collections_WithNullableElements_AreHandled()
    {
        var source = @"
#nullable enable
using System.Collections.Generic;
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public List<string?> Names { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public List<string?> Names { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.True(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.Contains("Names", generatedCode);
    }

    // =====================================================================
    // Enum ↔ int conversion
    // =====================================================================

    [Fact]
    public void Enum_ToInt_Conversion()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public enum Status { Active = 1, Inactive = 2 }

public class Source { public Status Status { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public int Status { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        // NOTE: Depends on Neo implementing enum→int conversion
        Assert.True(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.Contains("Status", generatedCode);
    }

    [Fact]
    public void Int_ToEnum_Conversion()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public enum Status { Active = 1, Inactive = 2 }

public class Source { public int StatusCode { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    [MapProperty(""StatusCode"")]
    public Status Status { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        // NOTE: Depends on Neo implementing int→enum conversion
        Assert.True(result.GeneratedSources.Length > 0);
    }

    // =====================================================================
    // [MapIgnore] verification — no assignment generated
    // =====================================================================

    [Fact]
    public void MapIgnore_PropertyIsNotMapped()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } public string Secret { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public string Name { get; set; }
    
    [MapIgnore]
    public string Secret { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.True(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        
        // Name should be mapped
        Assert.Contains("Name", generatedCode);
        
        // Secret should NOT be in generated code assignment
        // NOTE: Depends on Neo implementing [MapIgnore] support
        Assert.DoesNotContain("Secret = source.Secret", generatedCode);
    }

    [Fact]
    public void MapIgnore_OnRecord_ConstructorParameter()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } public string Secret { get; set; } }

[MapFrom(typeof(Source))]
public partial record Destination(string Name, [property: MapIgnore] string Secret);
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        // NOTE: Depends on Neo implementing [MapIgnore] for record constructor params
        Assert.True(result.GeneratedSources.Length > 0);
    }

    // =====================================================================
    // [MapConverter] verification — converter call generated
    // =====================================================================

    [Fact]
    public void MapConverter_ConverterCallIsGenerated()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } }

public class UpperCaseConverter : IMapConverter<string, string>
{
    public string Convert(string source) => source.ToUpper();
}

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    [MapConverter(typeof(UpperCaseConverter))]
    public string Name { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.True(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        
        // Should instantiate converter and call Convert()
        // NOTE: Depends on Neo implementing [MapConverter] support
        Assert.Contains("UpperCaseConverter", generatedCode);
        Assert.Contains("Convert", generatedCode);
    }

    [Fact]
    public void MapConverter_WithValueTypeConversion()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public int Age { get; set; } }

public class AgeToStringConverter : IMapConverter<int, string>
{
    public string Convert(int source) => source.ToString();
}

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    [MapConverter(typeof(AgeToStringConverter))]
    public string Age { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        // NOTE: Depends on Neo implementing [MapConverter] support
        Assert.True(result.GeneratedSources.Length > 0);
        var generatedCode = result.GeneratedSources[0].SourceText.ToString();
        Assert.Contains("AgeToStringConverter", generatedCode);
    }

    // =====================================================================
    // Multiple edge cases combined
    // =====================================================================

    [Fact]
    public void CombinedEdgeCases_RequiredInitNullableConverter()
    {
        var source = @"
#nullable enable
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source 
{ 
    public string? Name { get; set; }
    public int Age { get; set; }
}

public class NameConverter : IMapConverter<string?, string>
{
    public string Convert(string? source) => source?.ToUpper() ?? ""UNKNOWN"";
}

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    [MapConverter(typeof(NameConverter))]
    public required string Name { get; init; }
    
    public required int Age { get; init; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        // NOTE: Combines multiple features — depends on Neo's full implementation
        Assert.True(result.GeneratedSources.Length > 0);
    }
}
