using Xunit;

namespace ElBruno.AotMapper.Generator.Tests;

public class NestedMappingTests
{
    [Fact]
    public void GeneratesMapping_ForNestedObjects()
    {
        var source = @"
using ElBruno.AotMapper;

namespace TestNamespace;

public class Address { public string Street { get; set; } public string City { get; set; } }
public class Person { public string Name { get; set; } public Address Address { get; set; } }

[MapFrom(typeof(Address))]
public partial class AddressDto { public string Street { get; set; } public string City { get; set; } }

[MapFrom(typeof(Person))]
public partial class PersonDto 
{ 
    public string Name { get; set; }
    public AddressDto Address { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.Empty(result.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error));
        Assert.NotEmpty(result.GeneratedSources);
        
        var generatedCode = string.Join("\n", result.GeneratedSources.Select(s => s.SourceText.ToString()));
        Assert.Contains("ToPersonDto", generatedCode);
        Assert.Contains("ToAddressDto", generatedCode);
    }

    [Fact]
    public void GeneratesMapping_ForNestedCollections()
    {
        var source = @"
using ElBruno.AotMapper;
using System.Collections.Generic;

namespace TestNamespace;

public class Item { public string Name { get; set; } }
public class Container { public List<Item> Items { get; set; } }

[MapFrom(typeof(Item))]
public partial class ItemDto { public string Name { get; set; } }

[MapFrom(typeof(Container))]
public partial class ContainerDto 
{ 
    public List<ItemDto> Items { get; set; }
}
";
        var (result, compilation) = Helpers.GeneratorTestHelper.RunGenerator(source);
        
        Assert.Empty(result.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error));
        Assert.NotEmpty(result.GeneratedSources);
    }
}
