# Morpheus — History

## Project Context
- **Project:** ElBruno.AotMapper — AOT-friendly DTO mapper for .NET using Roslyn source generators
- **Tech Stack:** .NET 8/10, C#, Roslyn Incremental Source Generators, xUnit, BenchmarkDotNet
- **User:** Bruno Capuano
- **PRD:** docs/AOT_Mapper_PRD.md

## Learnings

### 2025-01-XX: Initial Repository Scaffolding
Successfully scaffolded the complete repository structure for ElBruno.AotMapper:

**Key Architecture Decisions:**
- **Multi-package structure**: 4 NuGet packages (ElBruno.AotMapper core attributes, ElBruno.AotMapper.Generator, ElBruno.AotMapper.AspNetCore, ElBruno.AotMapper.EntityFramework)
- **Source generator package**: Targets netstandard2.0 (Roslyn requirement), packaged as analyzer
- **Core library**: Targets netstandard2.0 + net8.0 + net10.0 to support both the generator (needs netstandard2.0) and modern runtime features
- **Integration libraries**: Target net8.0 + net10.0 only (no netstandard2.0 needed)
- **Test projects**: Target net8.0 only for simplicity (CI runners may lack preview SDKs)

**Technical Implementation:**
- Created `.slnx` XML-based solution file with logical folders (`/src/`, `/src/tests/`, `/src/samples/`)
- Set up `Directory.Build.props` with shared MSBuild properties (nullable enabled, code analysis, repository info, NuGet metadata)
- Configured `global.json` with `rollForward: latestMajor` for flexibility across dev/CI environments
- Created GitHub Actions workflows:
  - `build.yml`: CI build on push/PR (restore → build → test with net8.0)
  - `publish.yml`: NuGet publish via OIDC trusted publishing (no API key secrets needed)
- Updated `.gitignore` with .NET patterns (bin/, obj/, nupkg, IDE files, test results)

**Source Generator Package Setup:**
The generator package requires special MSBuild configuration:
- `<TargetFramework>netstandard2.0</TargetFramework>` (Roslyn analyzer requirement)
- `<EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>` and `<IsRoslynComponent>true</IsRoslynComponent>`
- `<IncludeBuildOutput>false</IncludeBuildOutput>` (don't include DLL in lib folder)
- Pack the analyzer DLL into `analyzers/dotnet/cs` path
- Pack the core library DLL into `lib/netstandard2.0` path
- Reference `Microsoft.CodeAnalysis.CSharp` 4.8.0 with `PrivateAssets="all"`

**Testing Setup:**
- ElBruno.AotMapper.Tests: Basic xUnit tests for core attributes
- ElBruno.AotMapper.Generator.Tests: Source generator tests using `Microsoft.CodeAnalysis.CSharp.SourceGenerators.Testing` 1.1.2
- Created placeholder tests to verify build infrastructure works
- All 3 tests passing ✅

**Build Verification:**
- ✅ `dotnet restore` succeeded (warnings about package version constraints are acceptable)
- ✅ `dotnet build` succeeded (all 6 projects built across multiple frameworks)
- ✅ `dotnet test` succeeded (3 tests passed)

**Remaining Work:**
- Add actual NuGet logo image to `images/nuget_logo.png`
- Implement the actual source generator logic in `AotMapperGenerator.cs`
- Add comprehensive tests for mapping scenarios
- Create sample projects demonstrating usage
- Document the API in `docs/`

