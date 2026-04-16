# Squad Decisions

## Active Decisions

### 1. Multi-Package Architecture for ElBruno.AotMapper

**Status:** ✅ Implemented  
**Decider:** Morpheus (Lead)  
**Date:** 2025-01-XX

**Decision:** Implement a **multi-package architecture** with 4 NuGet packages:
- **ElBruno.AotMapper** (core) — Attributes & abstractions, targets `netstandard2.0;net8.0;net10.0`
- **ElBruno.AotMapper.Generator** (source generator) — Roslyn incremental generator, targets `netstandard2.0`
- **ElBruno.AotMapper.AspNetCore** — DI registration helpers, targets `net8.0;net10.0`
- **ElBruno.AotMapper.EntityFramework** — EF Core IQueryable projection helpers, targets `net8.0;net10.0`

**Rationale:** Minimal dependencies for core scenarios, optional integrations, follows .NET ecosystem patterns (similar to `Microsoft.Extensions.*`).

**Consequences:**
- ✅ Users have minimal dependencies for basic scenarios
- ✅ Clear separation of concerns
- ✅ Easier to version and update independently
- ❌ More complex to maintain (4 packages instead of 1)
- ❌ Users need to reference 2 packages minimum (core + generator)

---

### 2. Generator Implementation Architecture

**Status:** ✅ Implemented  
**Decider:** Neo (Generator Dev)  
**Date:** 2025-01-31

**Decision:** Incremental Roslyn source generator with:

**Core Attributes:**
1. `[MapFrom(Type)]` — Mark destination types
2. `[MapTo(Type)]` — Mark source types
3. `[MapProperty(string source, string dest)]` — Property renaming
4. `[MapIgnore]` — Skip property
5. `[MapConverter(Type)]` — Custom converter
6. `IMapConverter<TSource, TDest>` — Converter interface

**Generator Components:**
- `MappingParser` — Extracts mapping info, validates in strict mode
- `MappingEmitter` — Generates extension methods
- `DiagnosticDescriptors` — 5 analyzer rules (AOTMAP001-AOTMAP005)

**Supported Features:**
- Classes, records, structs
- Nested object mapping, collections (`List<T>`, `IEnumerable<T>`, arrays)
- Enum ↔ Enum, Enum ↔ String conversions
- Property renaming, strict mode, custom converters
- All generated code is AOT-safe (no reflection)

---

### 3. Integration Package Design

**Status:** ✅ Implemented  
**Decider:** Trinity (Integration Dev)  
**Date:** 2026-04-15

**Decisions:**

**ASP.NET Core Integration:** Minimal surface area — `AddAotMapper()` placeholder, mappers are static extensions.  
**Rationale:** Generated methods already static extensions; no runtime registration needed. `AddAotMapper()` provides consistency and future extensibility.

**EF Core Projection Strategy:** Generate `ProjectToXxx()` methods for `IQueryable<T>` with EF compatibility validation.  
**Rationale:** Queryable projection is the killer feature for ORMs; not all patterns translate to SQL.

**Sample Coverage:** Three samples covering distinct use cases:
- `MinimalApiSample` — Web API, DI, endpoints
- `NativeAotSample` — AOT compilation
- `EfProjectionSample` — ORM integration, SQL projection vs in-memory

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
