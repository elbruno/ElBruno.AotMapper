# AOT & Trimming Guarantees

ElBruno.AotMapper is designed from the ground up to be **NativeAOT-safe** and **trimming-friendly**. This document explains what that means and how to verify your mappings are AOT-compatible.

## What AOT-Safe Means

When you use ElBruno.AotMapper with NativeAOT or assembly trimming, the generated mapping code must meet strict constraints:

### ✅ What Works

- **No reflection** — All member access is direct property/field access, compiled into IL
- **No dynamic codegen** — No `Delegate.CreateDelegate`, `Expression.Compile`, or IL.Emit
- **No `Activator.CreateInstance` (except via `[MapConverter]`)** — Objects are constructed via direct `new` calls
- **No late-bound method calls** — All method invocations are resolved at build time
- **Explicit type dependencies** — All referenced types must be in the assembly or reachable via public API

### ❌ Known Limitations

- **Reflection-based mapping patterns** — Won't work; write an explicit converter instead
- **Dynamic property names** — Mapping target must be compile-time known
- **Unmapped enums in trimmed code** — If a mapped enum is trimmed, the mapping fails
- **EF Core patterns outside ProjectTo scope** — See [AOTMAP007](#aotmap007-ef-projection-pattern-not-translatable)

## Verifying AOT Compatibility Locally

### 1. Build the Project Normally

First, ensure the project builds without errors:

```bash
dotnet build
```

### 2. Verify with NativeAOT Publish

Run a NativeAOT publish to detect trimming/AOT issues:

```bash
dotnet publish -p:PublishAot=true
```

This command:
- Invokes the AOT compiler
- Detects all reflection, dynamic IL, and other non-AOT patterns
- Produces warnings and errors if trimming would break your code

**Fix any warnings** — they indicate your mapping won't survive trimming.

### 3. Run the Published Binary

After a successful publish, test the AOT binary:

```bash
./bin/Release/net8.0/win-x64/publish/MyApp
```

If it runs without errors, your mappings are AOT-safe.

## The CI Smoke Test

ElBruno.AotMapper's CI/CD pipeline includes a NativeAOT smoke test:

1. The `NativeAotSample` project builds normally
2. We publish it with `PublishAot=true`
3. We run the published executable
4. If all diagnostics pass and the binary runs, AOT is verified

This test catches accidental non-AOT patterns and ensures each release is NativeAOT-ready.

## Custom Converters with `[MapConverter]`

The `[MapConverter]` attribute allows custom conversion logic. To keep it AOT-compatible:

```csharp
[MapFrom(typeof(Order))]
public sealed partial record OrderDto
{
    public string Id { get; init; }
    
    [MapConverter(typeof(StringToDateTimeConverter))]
    public DateTime CreatedAt { get; init; }
}

// ✅ Converter must have a parameterless constructor
public class StringToDateTimeConverter : IMapConverter<string, DateTime>
{
    public DateTime Convert(string source)
    {
        return DateTime.Parse(source);
    }
}
```

The generated code will call:

```csharp
new StringToDateTimeConverter().Convert(sourceValue)
```

**Important:** Your converter class must:
- Have a **parameterless public constructor** (or default constructor)
- Have no static state that relies on reflection
- Be accessible (public or internal with `InternalsVisibleTo`)

## Diagnostics and AOT

Several diagnostics help catch AOT issues early:

| Diagnostic | Meaning | AOT Impact |
|-----------|---------|-----------|
| [AOTMAP006](docs/diagnostics.md#aotmap006) | Inaccessible member | Constructor/property not reachable → code gen fails |
| [AOTMAP007](docs/diagnostics.md#aotmap007) | EF pattern not translatable | Projection may fail at runtime |

Address these before publishing to NativeAOT.

## Assembly Trimming

When using assembly trimming (e.g., with `<PublishTrimmed>true</PublishTrimmed>`):

1. Mark your DTOs and entities with `[Preserve]` if they're defined in trimmed assemblies
2. Ensure your `[MapConverter]` types are not trimmed:
   ```xml
   <TrimmerRootAssembly Include="MyApp.Converters" />
   ```
3. Use root descriptors (`.rd.xml` or `TrimmerRootDescriptor.xml`) to preserve mapping types if needed

## Further Reading

- [Supported Mappings](supported-mappings.md) — What the generator can map
- [Troubleshooting](troubleshooting.md) — Common issues and fixes
- [Diagnostics Reference](diagnostics.md) — All diagnostic IDs
- [Microsoft: Trimming .NET applications](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/trim-self-contained)
- [Microsoft: NativeAOT deployment](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot)
