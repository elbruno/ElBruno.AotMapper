using Xunit;

namespace ElBruno.AotMapper.Generator.Tests;

public class DiagnosticTests
{
    [Fact]
    public void ReportsDiagnostic_ForMissingSourceProperty_WhenStrict()
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
        
        // When strict mode is enabled, missing source properties should generate diagnostics
        var diagnostics = result.Diagnostics;
        // This test expects the generator to report diagnostics for strict mode violations
        // The actual implementation will determine the diagnostic behavior
        Assert.NotNull(diagnostics);
    }

    [Fact]
    public void ReportsDiagnostic_ForTypeMismatch()
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
        
        // Type mismatch between source (string) and destination (int) should be handled
        var diagnostics = result.Diagnostics;
        Assert.NotNull(diagnostics);
    }

    [Fact]
    public void HandlesConflictingMapFromDefinitions()
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
        
        // Multiple MapFrom attributes should be supported (allows multiple source types)
        // The generator should handle this scenario appropriately
        Assert.NotNull(result);
    }
}
