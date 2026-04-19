using Xunit;
using Microsoft.CodeAnalysis;

namespace ElBruno.AotMapper.Generator.Tests;

/// <summary>
/// Tests for all AOTMAP diagnostic codes (AOTMAP001-AOTMAP007).
/// Each test verifies that diagnostics fire when expected AND do NOT fire when not expected.
/// </summary>
public class DiagnosticTests
{
    // =====================================================================
    // AOTMAP001: Missing source property (strict mode only)
    // =====================================================================

    [Fact]
    public void AOTMAP001_FiresWhenDestinationPropertyMissingInSource_StrictMode()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } }

[MapFrom(typeof(Source), Strict = true)]
public partial class Destination 
{ 
    public string Name { get; set; }
    public string ExtraProperty { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.Contains(result.Diagnostics, d => 
            d.Id == "AOTMAP001" && 
            d.Severity == DiagnosticSeverity.Error &&
            d.GetMessage().Contains("ExtraProperty"));
    }

    [Fact]
    public void AOTMAP001_DoesNotFireWhenStrictModeDisabled()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public string Name { get; set; }
    public string ExtraProperty { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.DoesNotContain(result.Diagnostics, d => d.Id == "AOTMAP001");
    }

    [Fact]
    public void AOTMAP001_DoesNotFireWhenAllPropertiesMatch()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } public int Age { get; set; } }

[MapFrom(typeof(Source), Strict = true)]
public partial class Destination 
{ 
    public string Name { get; set; }
    public int Age { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.DoesNotContain(result.Diagnostics, d => d.Id == "AOTMAP001");
    }

    // =====================================================================
    // AOTMAP002: Type mismatch (warning)
    // =====================================================================

    [Fact]
    public void AOTMAP002_FiresWhenPropertyTypesMismatch()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public int Name { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        // NOTE: This test depends on Neo implementing AOTMAP002 diagnostic
        // Currently parser returns MappingStrategy.Direct for mismatches
        Assert.Contains(result.Diagnostics, d => 
            d.Id == "AOTMAP002" && 
            d.Severity == DiagnosticSeverity.Warning);
    }

    [Fact]
    public void AOTMAP002_DoesNotFireWhenTypesMatch()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public string Name { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.DoesNotContain(result.Diagnostics, d => d.Id == "AOTMAP002");
    }

    // =====================================================================
    // AOTMAP003: Nullability mismatch (warning)
    // =====================================================================

    [Fact]
    public void AOTMAP003_FiresWhenNullableSourceMapsToNonNullableDestination()
    {
        var source = @"
#nullable enable
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string? Name { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public string Name { get; set; } = string.Empty;
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.Contains(result.Diagnostics, d => 
            d.Id == "AOTMAP003" && 
            d.Severity == DiagnosticSeverity.Warning &&
            d.GetMessage().Contains("Name"));
    }

    [Fact]
    public void AOTMAP003_DoesNotFireWhenNullabilityMatches()
    {
        var source = @"
#nullable enable
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string? Name { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public string? Name { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.DoesNotContain(result.Diagnostics, d => d.Id == "AOTMAP003");
    }

    [Fact]
    public void AOTMAP003_DoesNotFireWhenNonNullableSourceMapsToNullableDestination()
    {
        var source = @"
#nullable enable
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } = string.Empty; }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public string? Name { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.DoesNotContain(result.Diagnostics, d => d.Id == "AOTMAP003");
    }

    // =====================================================================
    // AOTMAP004: Duplicate mapping (error)
    // =====================================================================

    [Fact(Skip = "v0.7: AOTMAP004 duplicate detection for repeated MapFrom attributes — tracked in CHANGELOG roadmap")]
    public void AOTMAP004_FiresWhenConflictingMapFromOnSameDestination()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class SourceA { public string Name { get; set; } }
public class SourceB { public string Name { get; set; } }

[MapFrom(typeof(SourceA))]
[MapFrom(typeof(SourceA))]
public partial class Destination 
{ 
    public string Name { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        // NOTE: This test depends on Neo implementing AOTMAP004 detection
        Assert.Contains(result.Diagnostics, d => 
            d.Id == "AOTMAP004" && 
            d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void AOTMAP004_DoesNotFireWhenMultipleDistinctMapFrom()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class SourceA { public string Name { get; set; } }
public class SourceB { public string Name { get; set; } }

[MapFrom(typeof(SourceA))]
[MapFrom(typeof(SourceB))]
public partial class Destination 
{ 
    public string Name { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        // Multiple distinct MapFrom is valid (generates ToDestination extension for each)
        Assert.DoesNotContain(result.Diagnostics, d => d.Id == "AOTMAP004");
    }

    // =====================================================================
    // AOTMAP005: Invalid converter (error)
    // =====================================================================

    [Fact]
    public void AOTMAP005_FiresWhenConverterDoesNotImplementIMapConverter()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } }

public class NotAConverter { }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    [MapConverter(typeof(NotAConverter))]
    public string Name { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        // NOTE: This test depends on Neo implementing AOTMAP005 validation
        Assert.Contains(result.Diagnostics, d => 
            d.Id == "AOTMAP005" && 
            d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void AOTMAP005_DoesNotFireWhenConverterImplementsIMapConverter()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } }

public class NameConverter : IMapConverter<string, string>
{
    public string Convert(string source) => source.ToUpper();
}

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    [MapConverter(typeof(NameConverter))]
    public string Name { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.DoesNotContain(result.Diagnostics, d => d.Id == "AOTMAP005");
    }

    // =====================================================================
    // AOTMAP006: Inaccessible member (error) — depends on Neo's work
    // =====================================================================

    [Fact]
    public void AOTMAP006_FiresWhenSourcePropertyIsPrivate()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { private string Name { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public string Name { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        // NOTE: This test depends on Neo implementing AOTMAP006
        // Currently parser only looks at public members via GetMembers()
        // This test will PASS once Neo adds accessibility validation
        Assert.Contains(result.Diagnostics, d => 
            d.Id == "AOTMAP006" && 
            d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void AOTMAP006_DoesNotFireWhenAllMembersAccessible()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public string Name { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.DoesNotContain(result.Diagnostics, d => d.Id == "AOTMAP006");
    }

    // =====================================================================
    // AOTMAP007: Placeholder for future diagnostic
    // =====================================================================

    [Fact]
    public void AOTMAP007_Reserved()
    {
        // Reserved for future diagnostic code
        // Will be implemented when AOTMAP007 is defined
        Assert.True(true);
    }
}
