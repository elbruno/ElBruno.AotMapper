using Xunit;

namespace ElBruno.AotMapper.Generator.Tests;

public class BasicMappingTests
{
    [Fact]
    public void GeneratesMapping_ForSimpleClass()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } public int Age { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination { public string Name { get; set; } public int Age { get; set; } }
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.Empty(result.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error));
        Assert.NotEmpty(result.GeneratedSources);
        
        var generatedCode = result.GeneratedSources.First().SourceText.ToString();
        Assert.Contains("ToDestination", generatedCode);
        Assert.Contains("source.Name", generatedCode);
        Assert.Contains("source.Age", generatedCode);
    }

    [Fact]
    public void GeneratesMapping_ForRecord()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Id { get; set; } public string Name { get; set; } }

[MapFrom(typeof(Source))]
public partial record Destination(string Id, string Name);
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.Empty(result.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error));
        Assert.NotEmpty(result.GeneratedSources);
        
        var generatedCode = result.GeneratedSources.First().SourceText.ToString();
        Assert.Contains("ToDestination", generatedCode);
        Assert.Contains("new global::TestNamespace.Destination(", generatedCode);
    }

    [Fact]
    public void GeneratesMapping_ForStruct()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public struct Source { public int X { get; set; } public int Y { get; set; } }

[MapFrom(typeof(Source))]
public partial struct Destination { public int X { get; set; } public int Y { get; set; } }
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.Empty(result.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error));
        Assert.NotEmpty(result.GeneratedSources);
    }

    [Fact]
    public void GeneratesMapping_WithPropertyRename()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string FirstName { get; set; } }

[MapFrom(typeof(Source))]
[MapProperty(""FirstName"", ""Name"")]
public partial class Destination { public string Name { get; set; } }
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.Empty(result.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error));
        var generatedCode = result.GeneratedSources.First().SourceText.ToString();
        Assert.Contains("source.FirstName", generatedCode);
    }

    [Fact]
    public void GeneratesMapping_WithIgnoredProperty()
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
    public string Computed { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.Empty(result.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error));
        var generatedCode = result.GeneratedSources.First().SourceText.ToString();
        Assert.DoesNotContain("Computed", generatedCode);
    }

    [Fact]
    public void GeneratesCollectionMapping()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string Name { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination { public string Name { get; set; } }
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.NotEmpty(result.GeneratedSources);
        var generatedCode = result.GeneratedSources.First().SourceText.ToString();
        Assert.Contains("ToDestinationList", generatedCode);
    }

    [Fact]
    public void GeneratesMapping_WithNullableProperties()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Source { public string? Name { get; set; } public int? Age { get; set; } }

[MapFrom(typeof(Source))]
public partial class Destination { public string? Name { get; set; } public int? Age { get; set; } }
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.Empty(result.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error));
        Assert.NotEmpty(result.GeneratedSources);
    }
}
