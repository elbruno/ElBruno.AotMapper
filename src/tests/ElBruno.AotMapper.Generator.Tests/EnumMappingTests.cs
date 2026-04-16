using Xunit;

namespace ElBruno.AotMapper.Generator.Tests;

public class EnumMappingTests
{
    [Fact]
    public void GeneratesMapping_ForEnumToEnum()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public enum SourceStatus { Active, Inactive }
public enum DestStatus { Active, Inactive }

public class Source { public SourceStatus Status { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public DestStatus Status { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.Empty(result.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error));
        Assert.NotEmpty(result.GeneratedSources);
        
        var generatedCode = result.GeneratedSources.First().SourceText.ToString();
        Assert.Contains("ToDestination", generatedCode);
    }

    [Fact]
    public void GeneratesMapping_ForEnumToString()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public enum Status { Active, Inactive }

public class Source { public Status Status { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public string Status { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.Empty(result.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error));
        Assert.NotEmpty(result.GeneratedSources);
    }

    [Fact]
    public void GeneratesMapping_ForStringToEnum()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public enum Status { Active, Inactive }

public class Source { public string Status { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination 
{ 
    public Status Status { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.Empty(result.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error));
        Assert.NotEmpty(result.GeneratedSources);
    }
}
