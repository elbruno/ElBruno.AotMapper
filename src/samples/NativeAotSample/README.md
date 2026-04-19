# NativeAotSample

Demonstrates how to use ElBruno.AotMapper in a NativeAOT-compiled application. Published binaries are fully ahead-of-time compiled—no JIT, no reflection, maximum performance.

## Prerequisites

- .NET 8 SDK or later
- Platform-specific toolchain:
  - **Windows**: Visual Studio Build Tools or MSVC
  - **Linux**: `build-essential`, `clang`
  - **macOS**: Xcode Command Line Tools

## What It Demonstrates

- NativeAOT-compatible mapping patterns
- Zero-reflection generated code
- Publishing with `PublishAot=true`
- Running the compiled native binary
- Verifying AOT compatibility with diagnostics

## Key Files

- `Program.cs` — Console application, demonstrates mappers
- `Models/Product.cs` — Domain entity
- `Dtos/ProductDto.cs` — DTO with `[MapFrom]`
- `.csproj` — NativeAOT project configuration

## Build & Run

### Standard .NET Build

```bash
cd src/samples/NativeAotSample

# Restore and build normally
dotnet restore
dotnet build

# Run (uses JIT)
dotnet run
```

### NativeAOT Publish & Run

```bash
# Publish as NativeAOT
dotnet publish -c Release -p:PublishAot=true

# Run the native binary (no .NET runtime required!)
./bin/Release/net8.0/win-x64/publish/NativeAotSample.exe

# On Linux/macOS
./bin/Release/net8.0/linux-x64/publish/NativeAotSample
```

## What Happens

1. **Publish step:**
   - Compiler generates native machine code for all mapped types
   - Verifies zero reflection usage
   - Strips unused code
   - Produces a standalone binary (~60–80 MB depending on dependencies)

2. **Run step:**
   - Binary runs directly (no JIT, no GC pauses)
   - Cold startup is instant (< 10 ms)
   - Memory usage is minimal

## Expected Output

```
Mapping Product → ProductDto with NativeAOT
Product: { Id = 1, Name = "Widget", Price = 9.99 }
Mapped:  { Id = 1, Name = "Widget", Price = 9.99 }

✓ NativeAOT compilation successful
```

## Architecture

```
NativeAotSample
├── Program.cs              # Maps Product → ProductDto, displays result
├── Models/
│   └── Product.cs
├── Dtos/
│   └── ProductDto.cs       # [MapFrom(typeof(Product))]
└── NativeAotSample.csproj  # <PublishAot>true</PublishAot>
```

## Troubleshooting

### "Compilation error: Member X is not AOT-safe"

This means the mapped type uses reflection or dynamic patterns. Solutions:
- Use `[MapIgnore]` to skip unsupported properties
- Use `[MapConverter]` with a public, parameterless converter class
- Check [AOT & Trimming Guarantees](../../docs/aot.md) for detailed guidance

### "Native binary fails to start"

Verify the publish succeeded without warnings:

```bash
dotnet publish -c Release -p:PublishAot=true --no-build
```

Check for any IL warnings in the build output.

### "Binary size is unexpectedly large"

NativeAOT binaries include runtime libraries. For production, consider:
- Cross-compiling with `-r win-x64` (or your target platform)
- Using `-p:StripSymbols=true` to remove debug symbols

## Learn More

- [AOT & Trimming Guarantees](../../docs/aot.md)
- [Troubleshooting](../../docs/troubleshooting.md)
- [Microsoft: NativeAOT Deployment](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
