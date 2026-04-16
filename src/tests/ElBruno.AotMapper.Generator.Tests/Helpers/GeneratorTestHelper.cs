using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ElBruno.AotMapper.Generator.Tests.Helpers;

internal static class GeneratorTestHelper
{
    public static (GeneratorRunResult result, Compilation outputCompilation) RunGenerator(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToList();

        // Add reference to the core attributes assembly
        references.Add(MetadataReference.CreateFromFile(typeof(MapFromAttribute).Assembly.Location));

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithNullableContextOptions(NullableContextOptions.Enable));

        var generator = new AotMapperGenerator();
        
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
        
        var runResult = driver.GetRunResult();
        return (runResult.Results.Single(), outputCompilation);
    }
}
