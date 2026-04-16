# Trinity — History

## Project Context
- **Project:** ElBruno.AotMapper — AOT-friendly DTO mapper for .NET using Roslyn source generators
- **Tech Stack:** .NET 8/10, C#, ASP.NET Core, EF Core
- **User:** Bruno Capuano
- **PRD:** docs/AOT_Mapper_PRD.md

## Learnings

### Session: Integration Packages and Sample Applications
**Date:** 2026-04-15

**Completed:**
- ✅ Implemented `ElBruno.AotMapper.AspNetCore` package:
  - `AotMapperServiceCollectionExtensions` — DI registration for future scenarios
  - `MapperEndpointExtensions` — Placeholder for ASP.NET Core-specific utilities
- ✅ Implemented `ElBruno.AotMapper.EntityFramework` package:
  - `AotMapperQueryableExtensions` with `ApplyProjection<TSource, TDestination>()` helper
  - `EfCompatibilityNotes` documenting projection limitations (custom converters not supported in SQL)
- ✅ Created three sample applications:
  - `MinimalApiSample` — Minimal API with customer/address DTOs showing endpoint integration
  - `NativeAotSample` — Console app with `PublishAot=true` demonstrating AOT compatibility
  - `EfProjectionSample` — EF Core SQLite demo with both `ProjectToXxx()` and `ToXxx()` mapping
- ✅ Updated `ElBruno.AotMapper.slnx` to include all three samples under `/src/samples/` folder

**Technical Notes:**
- Integration packages built successfully (net8.0 target)
- Samples reference generator as `OutputItemType="Analyzer"` for proper source generation
- EF sample demonstrates both queryable projection (SQL) and in-memory mapping patterns
- NativeAot sample uses `InvariantGlobalization=true` for smaller binary size
- MinimalApi sample shows practical endpoint usage with `AddAotMapper()` DI pattern

**Decisions:**
- `AddAotMapper()` is a placeholder for future DI scenarios — current mappers are static extension methods
- EF projections require simple property mappings; complex converters trigger warnings and skip projection generation
- Samples target `net8.0` only (CI runners may not have preview SDKs)

